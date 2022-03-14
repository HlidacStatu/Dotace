// See https://aka.ms/new-console-template for more information

using System.Net.Http.Headers;
using Common;
using Common.IntermediateDb;
using Red;
using Red.Model;

var appLogger = Logging.CreateLogger("application.log");

appLogger.Debug("Start RED!");

appLogger.Debug("Loading configuration...");

string cnnString = DataHelper.GetDbConnectionString();

appLogger.Debug("Prepare dbs");

//intermediate db
await IntermediateDbContext.EnsureDbIsCreated(cnnString);

appLogger.Debug("Db was created");

var httpClient = new HttpClient();
httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
var clientWrapper = new HttpWrapper(httpClient);


appLogger.Debug("Creating data directory and downloading data");
string folder = @"data";
Directory.CreateDirectory(folder);

await FileHelpers.RemoveFiles(folder);
await Steps.DownloadFiles(folder, clientWrapper);

//todo: zrychlit pomocí Task all
var csvDotace = await CsvHelpers.LoadCsvAsDictionary<CsvDotace>(folder, d => d.Iridotace); // mohlo by byt enumeraci
var csvPrijemce = await CsvHelpers.LoadCsvAsDictionary<CsvPrijemce>(folder, d => d.Iriprijemce);
var csvAdresa = await CsvHelpers.LoadCsvAsDictionary<CsvAdresa>(folder, d => d.Iriprijemce);
var csvCiselnikOperacniProgram =
    await CsvHelpers.LoadCsvAsDictionary<CsvCiselnikoperacniprogram>(folder, d => d.IriOperacniProgram);
var csvRozhodnuti = await CsvHelpers.LoadCsvAsDictionaryList<CsvRozhodnuti>(folder, d => d.Iridotace);
var csvCisPoskytovatel =
    await CsvHelpers.LoadCsvAsDictionary<CsvCiselnikdotaceposkytovatel>(folder, d => d.Iridotaceposkytovatel);
var csvCisFinZdroj = await CsvHelpers.LoadCsvAsDictionary<CsvCiselnikfinancnizdroj>(folder, d => d.Irifinancnizdroj);
var csvRozpoctoveObdobi = await CsvHelpers.LoadCsvAsDictionaryList<CsvRozpoctoveobdobi>(folder, d => d.Irirozhodnuti);

// pospojování dat dohromady
var dotaceResults = new List<Dotace>();
foreach (var dotaceRec in csvDotace)
{
    var val = dotaceRec.Value;
    if (csvCiselnikOperacniProgram.TryGetValue(val.Irioperacniprogram, out var operacniProgramRec) == false)
    {
        appLogger.Warning($"U dotace [{val.Iridotace}] nenalezen operacni program [{val.Irioperacniprogram}]");
    }

    if (csvPrijemce.TryGetValue(val.Iriprijemce, out var prijemceRec) == false)
    {
        appLogger.Warning($"U dotace [{val.Iridotace}] nenalezen prijemce [{val.Iriprijemce}]");
    }

    if (csvAdresa.TryGetValue(val.Iriprijemce, out var adresaRec) == false)
    {
        appLogger.Warning($"U dotace [{val.Iridotace}] nenalezena adresa [{val.Iriprijemce}]");
    }

    var rozhodnutiPack = new List<Rozhodnuti>();
    if (csvRozhodnuti.TryGetValue(val.Iridotace, out var rozhodnutiRecs) == false)
    {
        appLogger.Warning($"U dotace [{val.Iridotace}] nenalezeno rozhodnuti [{val.Iridotace}]");
    }
    else
    {
        foreach (var rozhodnutiRec in rozhodnutiRecs)
        {
            if (csvCisPoskytovatel.TryGetValue(rozhodnutiRec.IriDotacePoskytovatel, out var poskytovatelRec) == false)
            {
                appLogger.Warning(
                    $"U rozhodnuti [{rozhodnutiRec.Irirozhodnuti}] nenalezen poskytovatel [{rozhodnutiRec.IriDotacePoskytovatel}]");
            }

            if (csvCisFinZdroj.TryGetValue(rozhodnutiRec.Irifinancnizdroj, out var finZdrojRec) == false)
            {
                appLogger.Warning(
                    $"U rozhodnuti [{rozhodnutiRec.Irirozhodnuti}] nenalezen financni zdroj [{rozhodnutiRec.Irifinancnizdroj}]");
            }

            var cerpaniPack = new List<Cerpani>();
            if (csvRozpoctoveObdobi.TryGetValue(rozhodnutiRec.Irirozhodnuti, out var rozpoctoveObdobiRecs) == false)
            {
                appLogger.Warning(
                    $"U rozhodnuti [{rozhodnutiRec.Irirozhodnuti}] nenalezeno rozpoctove obdobi [{rozhodnutiRec.Irirozhodnuti}]");
            }
            else
            {
                foreach (var rozpoctoveobdobiRec in rozpoctoveObdobiRecs)
                {
                    cerpaniPack.Add(new Cerpani()
                    {
                        Id = rozpoctoveobdobiRec.Irirozpoctoveobdobi,
                        Rok = rozpoctoveobdobiRec.Obdobi,
                        CastkaSpotrebovana = rozpoctoveobdobiRec.Castkaspotrebovana - rozpoctoveobdobiRec.Castkavracena
                    });
                }
            }

            rozhodnutiPack.Add(new Rozhodnuti()
            {
                Id = rozhodnutiRec.Irirozhodnuti,
                Rok = rozhodnutiRec.Rokrozhodnuti,
                JePujcka = rozhodnutiRec.Navratnostindikator,
                CastkaPozadovana = rozhodnutiRec.Castkapozadovana,
                CastkaRozhodnuta = rozhodnutiRec.Castkarozhodnuta,
                Poskytovatel = poskytovatelRec?.Dotaceposkytovatelnazev ?? "",
                ZdrojFinanci = finZdrojRec?.Financnizdrojnazev ?? "",
                Cerpani = cerpaniPack,
            });
        }
    }

    Dotace dotace = new()
    {
        Id = $"ISRED-{val.Iridotace}",
        IdDotace = val.Iridotace,
        DatumAktualizace = val.Datumaktualizace,
        DatumPodpisu = val.Podpisdatum,
        KodProjektu = val.Identifikator,
        NazevProjektu = val.Nazev.Replace("\0", ""), // replace null character
        ProgramNazev = operacniProgramRec?.OperacniProgramNazev,
        ProgramKod = operacniProgramRec?.OperacaniProgramKod,
        PrijemceIco = prijemceRec?.Ico,
        PrijemceObchodniJmeno = prijemceRec?.Obchodninazev,
        PrijemceJmeno = (prijemceRec?.Jmeno + " " + prijemceRec?.Prijmeni).Trim(),
        PrijemceRokNarozeni = prijemceRec?.Roknarozeni,
        PrijemceObec = adresaRec?.Obecnazev,
        PrijemceOkres = adresaRec?.Okres,
        PrijemceUlice = $"{adresaRec?.Ulice} {adresaRec?.Cislodomovni}".Trim(),
        PrijemcePSC = adresaRec?.Psc,
        ZdrojNazev = "IS Red",
        ZdrojUrl = "https://red.financnisprava.cz/registr-dotaci/prijemci",
        Rozhodnuti = rozhodnutiPack
    };
    dotaceResults.Add(dotace);
}

appLogger.Debug("Uploading dotace to db");

await DotaceRepo.SaveDotaceToDb(dotaceResults, appLogger, cnnString);
appLogger.Debug("Finished");