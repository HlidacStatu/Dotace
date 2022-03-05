using System.Net.Http.Headers;
using System.Text.Json;
using CedrIs;
using CedrIs.Model;
using Common;
using Common.IntermediateDb;

var appLogger = Common.Logging.CreateLogger("application.log");
appLogger.Debug("Start RED!");
appLogger.Debug("Loading configuration...");

var configFile = File.OpenRead("appsettings.json");
var config = await JsonSerializer.DeserializeAsync<Dictionary<string, string>>(configFile);
if (config == null || config.Count == 0) throw new ArgumentNullException(nameof(config));
var dbIntermediateCnnString = config["dbintermediate"];

// jak to udělat s databází?
// appLogger.Debug("Prepare db");
// //intermediate db
// await using (var db = new IntermediateDbContext(dbIntermediateCnnString))
// {
//     await db.Database.EnsureDeletedAsync();
//     await db.Database.EnsureCreatedAsync();
// }

appLogger.Debug("Db was created");

var httpClient = new HttpClient();
httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
var clientWrapper = new HttpWrapper(httpClient);


appLogger.Debug("Creating data directory and downloading data");
string folder = @"data";
Directory.CreateDirectory(folder);

// await FileHelpers.RemoveFiles(folder);
// await Steps.DownloadFiles(folder, clientWrapper);


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

var csvAdresaSidlo = await CsvHelpers.LoadCsvAsDictionary<CsvAdresa>(folder, d => d.IdPrijemce, "adresasidlo.csv.gz");
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

var csvAdresaBydliste = await CsvHelpers.LoadCsvAsDictionary<CsvAdresa>(folder, d => d.IdPrijemce, "adresabydliste.csv.gz");
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

var csvAdresa = csvAdresaSidlo.Concat(csvAdresaBydliste);

csvAdresaSidlo = null;
csvAdresaBydliste = null;




Console.WriteLine("stopp");
//var csvDotace = await CsvHelpers.LoadCsvAsDictionary<CsvDotace>(folder, d => d.Iridotace); // mohlo by byt enumeraci







// pospojování dat dohromady
// var dotaceResults = new List<Common.IntermediateDb.Dotace>();
// foreach (var dotaceRec in csvDotace)
// {
//     var val = dotaceRec.Value;
//     if (csvCiselnikOperacniProgram.TryGetValue(val.Irioperacniprogram, out var operacniProgramRec) == false)
//     {
//         appLogger.Warning($"U dotace [{val.Iridotace}] nenalezen operacni program [{val.Irioperacniprogram}]");    
//     }
//
//     if (csvPrijemce.TryGetValue(val.Iriprijemce, out var prijemceRec) == false)
//     {
//         appLogger.Warning($"U dotace [{val.Iridotace}] nenalezen prijemce [{val.Iriprijemce}]");
//     }
//
//     if (csvAdresa.TryGetValue(val.Iriprijemce, out var adresaRec) == false)
//     {
//         appLogger.Warning($"U dotace [{val.Iridotace}] nenalezena adresa [{val.Iriprijemce}]");
//     }
//     
//     var rozhodnutiPack = new List<Common.IntermediateDb.Rozhodnuti>();
//     if (csvRozhodnuti.TryGetValue(val.Iridotace, out var rozhodnutiRecs) == false)
//     {
//         appLogger.Warning($"U dotace [{val.Iridotace}] nenalezeno rozhodnuti [{val.Iridotace}]");
//     }
//     else
//     {
//         foreach (var rozhodnutiRec in rozhodnutiRecs)
//         {
//             if (csvCisPoskytovatel.TryGetValue(rozhodnutiRec.IriDotacePoskytovatel, out var poskytovatelRec) == false)
//             {
//                 appLogger.Warning($"U rozhodnuti [{rozhodnutiRec.Irirozhodnuti}] nenalezen poskytovatel [{rozhodnutiRec.IriDotacePoskytovatel}]");
//             }
//
//             if (csvCisFinZdroj.TryGetValue(rozhodnutiRec.Irifinancnizdroj, out var finZdrojRec) == false)
//             {
//                 appLogger.Warning($"U rozhodnuti [{rozhodnutiRec.Irirozhodnuti}] nenalezen financni zdroj [{rozhodnutiRec.Irifinancnizdroj}]");
//             }
//             
//             var cerpaniPack = new List<Common.IntermediateDb.Cerpani>();
//             if (csvRozpoctoveObdobi.TryGetValue(rozhodnutiRec.Irirozhodnuti, out var rozpoctoveObdobiRecs) == false)
//             {
//                 appLogger.Warning($"U rozhodnuti [{rozhodnutiRec.Irirozhodnuti}] nenalezeno rozpoctove obdobi [{rozhodnutiRec.Irirozhodnuti}]");
//             }
//             else
//             {
//                 foreach (var rozpoctoveobdobiRec in rozpoctoveObdobiRecs)
//                 {
//                     cerpaniPack.Add(new Common.IntermediateDb.Cerpani()
//                     {
//                         Id = rozpoctoveobdobiRec.Irirozpoctoveobdobi,
//                         Rok = rozpoctoveobdobiRec.Obdobi,
//                         CastkaSpotrebovana = rozpoctoveobdobiRec.Castkaspotrebovana - rozpoctoveobdobiRec.Castkavracena
//                     });
//                 }
//             }
//
//             rozhodnutiPack.Add(new Common.IntermediateDb.Rozhodnuti()
//             {
//                 Id = rozhodnutiRec.Irirozhodnuti,
//                 Rok = rozhodnutiRec.Rokrozhodnuti,
//                 JePujcka = rozhodnutiRec.Navratnostindikator,
//                 CastkaPozadovana = rozhodnutiRec.Castkapozadovana,
//                 CastkaRozhodnuta = rozhodnutiRec.Castkarozhodnuta,
//                 Poskytovatel = poskytovatelRec?.Dotaceposkytovatelnazev ?? "",
//                 ZdrojFinanci = finZdrojRec?.Financnizdrojnazev ?? "",
//                 Cerpani = cerpaniPack,
//             });
//         }
//     }
//     
//     
//     Common.IntermediateDb.Dotace dotace = new()
//     {
//         Id = $"ISRED-{val.Iridotace.Split('/').Last()}",
//         IdDotace = val.Iridotace,
//         DatumAktualizace = val.Datumaktualizace,
//         DatumPodpisu = val.Podpisdatum,
//         KodProjektu = val.Identifikator,
//         NazevProjektu = val.Nazev,
//         ProgramNazev = operacniProgramRec?.OperacniProgramNazev,
//         ProgramKod = operacniProgramRec?.OperacaniProgramKod,
//         PrijemceIco = prijemceRec?.Ico,
//         PrijemceObchodniJmeno = prijemceRec?.Obchodninazev,
//         PrijemceJmeno = (prijemceRec?.Jmeno + " " + prijemceRec?.Prijmeni).Trim(),
//         PrijemceRokNarozeni = prijemceRec?.Roknarozeni,
//         PrijemceObec = adresaRec?.Obecnazev,
//         PrijemceOkres = adresaRec?.Okres,
//         PrijemceUlice = adresaRec?.Ulice,
//         PrijemcePSC = adresaRec?.Psc,
//         PrijemceCisloDomovni = adresaRec?.Cislodomovni,
//         ZdrojNazev = "IS Red",
//         ZdrojUrl = "https://red.financnisprava.cz/registr-dotaci/prijemci",
//         Rozhodnuti = rozhodnutiPack
//
//
//     };
//     dotaceResults.Add(dotace);
// }
// appLogger.Debug("check memory please");
//
// appLogger.Debug("Uploading dotace to db");
//
// var dotaceChunks = dotaceResults.Chunk(1000);
//
// int chunkNumber = 0;
// foreach (var recordChunk in dotaceChunks)
// {
//     appLogger.Debug($"Uploading chunk nbr {chunkNumber++}");
//
//     await using var db = new IntermediateDbContext(dbIntermediateCnnString);
//             
//     db.Dotace.AddRange(recordChunk);
//     await db.SaveChangesAsync();
// }
