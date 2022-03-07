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

var csvProgram = await CsvHelpers.LoadCsvAsDictionary<CsvCiselnikProgram>(folder, d => d.Id, "ciselnikProgramv01.csv.gz");

var tmpOp1 = await CsvHelpers.LoadCsvAsDictionary<CsvCiselnikOperacniProgram>(folder, d => d.IdOperacniProgram, "ciselnikMmrOperacniProgramv01.csv.gz");
var tmpOp2 = await CsvHelpers.LoadCsvAsDictionary<CsvCiselnikOperacniProgram>(folder, d => d.IdOperacniProgram, "ciselnikCedrOperacniProgramv01.csv.gz");
var csvOperacniProgramMerged = tmpOp1.Concat(tmpOp2);
tmpOp1 = null;
tmpOp2 = null;

var tmpOpatreni1 = await CsvHelpers.LoadCsvAsDictionary<CsvCiselnikOpatreni>(folder, d => d.IdOpatreni, "ciselnikMmrOpatreniv01.csv.gz");
var tmpOpatreni2 = await CsvHelpers.LoadCsvAsDictionary<CsvCiselnikOpatreni>(folder, d => d.IdOpatreni, "ciselnikCedrOpatreniv01.csv.gz");
var csvOpatreniMerged = tmpOpatreni1.Concat(tmpOpatreni2);
tmpOpatreni1 = null;
tmpOpatreni2 = null;

var tmpGs1 = await CsvHelpers.LoadCsvAsDictionary<CsvCiselnikGrantoveSchema>(folder, d => d.IdGrantoveSchema, "ciselnikMmrGrantoveSchemav01.csv.gz");
var tmpGs2 = await CsvHelpers.LoadCsvAsDictionary<CsvCiselnikGrantoveSchema>(folder, d => d.IdGrantoveSchema, "ciselnikCedrGrantoveSchemav01.csv.gz");
var csvGrantoveSchemaMerged = tmpGs1.Concat(tmpGs2);
tmpGs1 = null;
tmpGs2 = null;

var csvPrijemce = await CsvHelpers.LoadCsvAsDictionary<CsvPrijemce>(folder, d => d.IdPrijemce, "PrijemcePomoci.csv.gz");
var csvRozhodnuti = await CsvHelpers.LoadCsvAsDictionaryList<CsvRozhodnuti>(folder, d => d.IdDotace, "Rozhodnuti.csv.gz");
var csvCisPoskytovatel =
    await CsvHelpers.LoadCsvAsDictionary<CsvCiselnikdotaceposkytovatel>(folder, d => d.Id, "ciselnikDotacePoskytovatelv01.csv.gz");
var csvCisFinZdroj = await CsvHelpers.LoadCsvAsDictionary<CsvCiselnikfinancnizdroj>(folder, d => d.Id);
var csvRozpoctoveObdobi = await CsvHelpers.LoadCsvAsDictionaryList<CsvRozpoctoveobdobi>(folder, d => d.IdRozhodnuti, "RozpoctoveObdobi.csv.gz");

var csvDotace = await CsvHelpers.LoadCsvAsDictionary<CsvDotace>(folder, d => d.Iridotace); // mohlo by byt enumeraci


Console.WriteLine("stopp");


