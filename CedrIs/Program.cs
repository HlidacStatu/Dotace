using System.Net.Http.Headers;
using System.Text.Json;
using CedrIs;
using CedrIs.Model;
using Common;
using Common.IntermediateDb;

var appLogger = Common.Logging.CreateLogger("application.log");
appLogger.Debug("Start RED!");
appLogger.Debug("Loading configuration...");

string cnnString = DataHelper.GetDbConnectionString();

appLogger.Debug("Prepare dbs");

//intermediate db
await using (var intermediateDbContext = new IntermediateDbContext(cnnString))
{
    await intermediateDbContext.Database.EnsureCreatedAsync();
}

appLogger.Debug("Db was created");

var httpClient = new HttpClient();
httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
var clientWrapper = new HttpWrapper(httpClient);


appLogger.Debug("Creating data directory and downloading data");
string folder = @"data";
Directory.CreateDirectory(folder);

await FileHelpers.RemoveFiles(folder);
await Steps.DownloadFiles(folder, clientWrapper);


//spojení adres
var csvObec = await CsvHelpers.LoadCsvAsDictionary<CsvCiselnikObec>(folder, d => d.Id, "ciselnikObecv01.csv.gz");
var csvOkres = await CsvHelpers.LoadCsvAsDictionaryList<CsvCiselnikOkres>(folder, d => d.OkresKod, "ciselnikOkresv01.csv.gz");
foreach (var rec in csvObec)
{
    var val = rec.Value;
    string okresKod = val.OkresNad.Split('/').Last();
    if (csvOkres.TryGetValue(okresKod, out var okresRecs) == false)
    {
        appLogger.Warning($"U obce [{val.Id}] nenalezen okres [{val.OkresNad}]");
        continue;
    }
    
    val.Okres = okresRecs.FirstOrDefault()?.OkresNazev ?? ""; 
}
csvOkres = null;

var csvAdresaSidlo = await CsvHelpers.LoadCsvAsDictionary<CsvAdresa>(folder, d => d.IdPrijemce, "AdresaSidlo.csv.gz");
foreach (var rec in csvAdresaSidlo)
{
    var val = rec.Value;
     if (csvObec.TryGetValue(val.Iriobec, out var obecRec) == false)
     {
         appLogger.Warning($"U adresy sidla [{val.IdPrijemce}] nenalezena obec [{val.Iriobec}]");
         continue;
     }
    
    val.Okres = obecRec.Okres; 
}

var csvAdresaBydliste = await CsvHelpers.LoadCsvAsDictionary<CsvAdresa>(folder, d => d.IdPrijemce, "AdresaBydliste.csv.gz");
foreach (var rec in csvAdresaBydliste)
{
    var val = rec.Value;
    if (csvObec.TryGetValue(val.Obec, out var obecRec) == false)
    {
        appLogger.Warning($"U adresy bydliste [{val.IdPrijemce}] nenalezena obec [{val.Iriobec}]");
        continue;
    }

    val.Obecnazev = obecRec.ObecNazev;
    val.Okres = obecRec.Okres;
}

var csvAdresa = csvAdresaSidlo.Concat(csvAdresaBydliste).ToDictionary( k => k.Key, v => v.Value);

csvAdresaSidlo = null;
csvAdresaBydliste = null;

var csvProgram = await CsvHelpers.LoadCsvAsDictionary<CsvCiselnikProgram>(folder, d => d.Id, "ciselnikProgramv01.csv.gz");

var tmpOp1 = await CsvHelpers.LoadCsvAsDictionary<CsvCiselnikOperacniProgram>(folder, d => d.IdOperacniProgram, "ciselnikMmrOperacniProgramv01.csv.gz");
var tmpOp2 = await CsvHelpers.LoadCsvAsDictionary<CsvCiselnikOperacniProgram>(folder, d => d.IdOperacniProgram, "ciselnikCedrOperacniProgramv01.csv.gz");
var csvOperacniProgramMerged = tmpOp1.Concat(tmpOp2).ToDictionary( k => k.Key, v => v.Value);
tmpOp1 = null;
tmpOp2 = null;

var tmpOpatreni1 = await CsvHelpers.LoadCsvAsDictionary<CsvCiselnikOpatreni>(folder, d => d.IdOpatreni, "ciselnikMmrOpatreniv01.csv.gz");
var tmpOpatreni2 = await CsvHelpers.LoadCsvAsDictionary<CsvCiselnikOpatreni>(folder, d => d.IdOpatreni, "ciselnikCedrOpatreniv01.csv.gz");
var csvOpatreniMerged = tmpOpatreni1.Concat(tmpOpatreni2).ToDictionary( k => k.Key, v => v.Value);
tmpOpatreni1 = null;
tmpOpatreni2 = null;

var tmpGs1 = await CsvHelpers.LoadCsvAsDictionary<CsvCiselnikGrantoveSchema>(folder, d => d.IdGrantoveSchema, "ciselnikMmrGrantoveSchemav01.csv.gz");
var tmpGs2 = await CsvHelpers.LoadCsvAsDictionary<CsvCiselnikGrantoveSchema>(folder, d => d.IdGrantoveSchema, "ciselnikCedrGrantoveSchemav01.csv.gz");
var csvGrantoveSchemaMerged = tmpGs1.Concat(tmpGs2).ToDictionary( k => k.Key, v => v.Value);
tmpGs1 = null;
tmpGs2 = null;

var csvPrijemce = await CsvHelpers.LoadCsvAsDictionary<CsvPrijemce>(folder, d => d.IdPrijemce, "PrijemcePomoci.csv.gz");
var csvRozhodnuti = await CsvHelpers.LoadCsvAsDictionaryList<CsvRozhodnuti>(folder, d => d.IdDotace, "Rozhodnuti.csv.gz");
var csvCisPoskytovatel =
    await CsvHelpers.LoadCsvAsDictionary<CsvCiselnikdotaceposkytovatel>(folder, d => d.Id, "ciselnikDotacePoskytovatelv01.csv.gz");
var csvCisFinZdroj = await CsvHelpers.LoadCsvAsDictionary<CsvCiselnikfinancnizdroj>(folder, d => d.Id, "ciselnikFinancniZdrojv01.csv.gz");
var csvRozpoctoveObdobi = await CsvHelpers.LoadCsvAsDictionaryList<CsvRozpoctoveobdobi>(folder, d => d.IdRozhodnuti, "RozpoctoveObdobi.csv.gz");

var csvDotace = await CsvHelpers.LoadCsvAsDictionary<CsvDotace>(folder, d => d.IdDotace, "Dotace.csv.gz"); // mohlo by byt enumeraci


appLogger.Debug("Merging data");
var dotaceResults = new List<Dotace>();
foreach (var dotaceRec in csvDotace)
{
    var val = dotaceRec.Value;
    if (csvProgram.TryGetValue(val.IriProgram, out var programRec) == false)
    {
        appLogger.Warning($"U dotace [{val.IdDotace}] nenalezen program [{val.IriProgram}]");
    }
    
    if (csvOperacniProgramMerged.TryGetValue(val.IriOperacniProgram, out var operacniProgramRec) == false)
    {
        appLogger.Warning($"U dotace [{val.IdDotace}] nenalezen operacni program [{val.IriProgram}]");
    }
    
    if (csvOpatreniMerged.TryGetValue(val.IriOpatreni, out var opatreniRec) == false)
    {
        appLogger.Warning($"U dotace [{val.IdDotace}] nenalezeno opatreni [{val.IriOpatreni}]");
    }
    
    if (csvGrantoveSchemaMerged.TryGetValue(val.IriGrantoveSchema, out var grantoveSchemaRec) == false)
    {
        appLogger.Warning($"U dotace [{val.IdDotace}] nenalezeno grantove schema [{val.IriGrantoveSchema}]");
    }

    if (csvPrijemce.TryGetValue(val.IdPrijemce, out var prijemceRec) == false)
    {
        appLogger.Warning($"U dotace [{val.IdDotace}] nenalezen prijemce [{val.IdPrijemce}]");
    }

    if (csvAdresa.TryGetValue(val.IdPrijemce, out var adresaRec) == false)
    {
        appLogger.Warning($"U dotace [{val.IdDotace}] nenalezena adresa [{val.IdPrijemce}]");
    }

    var rozhodnutiPack = new List<Rozhodnuti>();
    if (csvRozhodnuti.TryGetValue(val.IdDotace, out var rozhodnutiRecs) == false)
    {
        appLogger.Warning($"U dotace [{val.IdDotace}] nenalezeno rozhodnuti [{val.IdDotace}]");
    }
    else
    {
        foreach (var rozhodnutiRec in rozhodnutiRecs)
        {
            if (csvCisPoskytovatel.TryGetValue(rozhodnutiRec.IriPoskytovatelDotace, out var poskytovatelRec) == false)
            {
                appLogger.Warning(
                    $"U rozhodnuti [{rozhodnutiRec.IdRozhodnuti}] nenalezen poskytovatel [{rozhodnutiRec.IriPoskytovatelDotace}]");
            }

            if (csvCisFinZdroj.TryGetValue(rozhodnutiRec.IriFinancniZdroj, out var finZdrojRec) == false)
            {
                appLogger.Warning(
                    $"U rozhodnuti [{rozhodnutiRec.IdRozhodnuti}] nenalezen financni zdroj [{rozhodnutiRec.IriFinancniZdroj}]");
            }

            var cerpaniPack = new List<Cerpani>();
            if (csvRozpoctoveObdobi.TryGetValue(rozhodnutiRec.IdRozhodnuti, out var rozpoctoveObdobiRecs) == false)
            {
                appLogger.Warning(
                    $"U rozhodnuti [{rozhodnutiRec.IdRozhodnuti}] nenalezeno rozpoctove obdobi [{rozhodnutiRec.IdRozhodnuti}]");
            }
            else
            {
                foreach (var rozpoctoveobdobiRec in rozpoctoveObdobiRecs)
                {
                    cerpaniPack.Add(new Cerpani()
                    {
                        Id = rozpoctoveobdobiRec.IdObdobi,
                        Rok = rozpoctoveobdobiRec.RozpoctoveObdobi,
                        CastkaSpotrebovana = rozpoctoveobdobiRec.Castkaspotrebovana - rozpoctoveobdobiRec.Castkavracena
                    });
                }
            }

            rozhodnutiPack.Add(new Rozhodnuti()
            {
                Id = rozhodnutiRec.IdRozhodnuti,
                Rok = rozhodnutiRec.RokRozhodnuti,
                JePujcka = rozhodnutiRec.NavratnostIndikator,
                CastkaPozadovana = rozhodnutiRec.CastkaPozadovana,
                CastkaRozhodnuta = rozhodnutiRec.CastkaRozhodnuta,
                Poskytovatel = poskytovatelRec?.DotacePoskytovatelNazev ?? "",
                ZdrojFinanci = finZdrojRec?.FinancniZdrojNazev ?? "",
                Cerpani = cerpaniPack,
            });
        }
    }

    string? programNazev = programRec?.ProgramNazev;
    string? programKod = programRec?.ProgramKod;
    if (string.IsNullOrWhiteSpace(programNazev))
    {
        programNazev = operacniProgramRec?.OperacniProgramNazev;
        programKod = operacniProgramRec?.OperacniProgramKod;
    }
    if (string.IsNullOrWhiteSpace(programNazev))
    {
        programNazev = opatreniRec?.OpatreniNazev;
        programKod = opatreniRec?.OpatreniKod;
    }
    if (string.IsNullOrWhiteSpace(programNazev))
    {
        programNazev = grantoveSchemaRec?.GrantoveSchemaNazev;
        programKod = grantoveSchemaRec?.GrantoveSchemaKod;
    }

    Dotace dotace = new()
    {
        Id = $"CEDR-{val.IdDotace}",
        IdDotace = val.IdDotace,
        DatumAktualizace = val.DtAktualizace,
        DatumPodpisu = val.PodpisDatum,
        KodProjektu = val.ProjektIdnetifikator,
        NazevProjektu = val.ProjektNazev.Replace("\0", ""), //replace null character (PGSQL issue)
        ProgramNazev = programNazev,
        ProgramKod = programKod,
        PrijemceIco = prijemceRec?.Ico,
        PrijemceObchodniJmeno = prijemceRec?.ObchodniJmeno,
        PrijemceJmeno = (prijemceRec?.Jmeno + " " + prijemceRec?.Prijmeni).Trim(),
        PrijemceRokNarozeni = prijemceRec?.RokNarozeni,
        PrijemceObec = adresaRec?.Obecnazev,
        PrijemceOkres = adresaRec?.Okres,
        PrijemcePSC = adresaRec?.Psc,
        ZdrojNazev = "CEDR",
        ZdrojUrl = $"http://cedropendata.mfcr.cz/c3lod/cedr/resource/Dotace/{val.IdDotace}",
        Rozhodnuti = rozhodnutiPack
    };
    dotaceResults.Add(dotace);
}

appLogger.Debug("Uploading dotace to db");

await DotaceRepo.SaveDotaceToDb(dotaceResults, appLogger, cnnString);
appLogger.Debug("Finished");