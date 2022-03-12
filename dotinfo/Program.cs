using System.Text.Json;
using Common;
using Common.IntermediateDb;
using DotInfo;
using DotInfo.Model;
using Microsoft.EntityFrameworkCore;

var appLogger = Logging.CreateLogger("application.log");

appLogger.Debug("Start Eufondy!");
appLogger.Debug("Loading configuration...");

var configFile = File.OpenRead("appsettings.json");
var config = await JsonSerializer.DeserializeAsync<Dictionary<string, string>>(configFile);
if (config == null || config.Count == 0) throw new ArgumentNullException(nameof(config));
var dbIntermediateCnnString = config["dbintermediate"];
var dotInfoCnnString = config["dbdotinfo"];


var dotaceResults = new List<Dotace>();

await using var db = new DotInfoDbContext(dotInfoCnnString);
var dotinfo = await db.DotInfo.ToListAsync();

foreach (var dotaceRec in dotinfo)
{
    List<Rozhodnuti> rozhodnuti = new()
    {
        new Rozhodnuti()
        {
            CastkaRozhodnuta = DataHelper.GetPriceFromText(dotaceRec.DotaceCastkaSchvalena),
            CastkaPozadovana = DataHelper.GetPriceFromText(dotaceRec.DotaceCastkaPozadovana),
            Poskytovatel = dotaceRec.PoskytovatelPoskytovatelNazevOs ?? "",
            IcoPoskytovatele = dotaceRec.PoskytovatelIcPoskytovatele ?? "",
            JePujcka = dotaceRec.DotaceFormaFinancovaniDotace?.ToUpper() == "NFV"
        }
    };


    var id = dotaceRec.Url?.Split("/").Last();
    var kodProjektu = string.IsNullOrWhiteSpace(dotaceRec.DotaceEvidencniCisloDotace)
        ? dotaceRec.KodProjektu
        : dotaceRec.DotaceEvidencniCisloDotace;
    var obchodniJmeno = string.IsNullOrWhiteSpace(dotaceRec.UcastnikObchodniJmeno)
        ? dotaceRec.UcastnikPrijemceDotaceJmeno
        : dotaceRec.UcastnikObchodniJmeno;

    bool dateFound = DateTime.TryParse(dotaceRec.DotaceDatumVydaniRozhodnuti, out var datumPodpisu);


    Dotace dotace = new()
    {
        Id = $"dotinfo-{id}",
        IdDotace = id,
        DatumPodpisu = dateFound ? datumPodpisu : null,
        NazevProjektu = dotaceRec.DotaceNazevDotace,
        KodProjektu = kodProjektu,
        ProgramKod = dotaceRec.DotaceIdentifikatorDotKodIs,
        PrijemceObchodniJmeno = obchodniJmeno,
        PrijemceIco = dotaceRec.UcastnikIcUcastnikaIcZahranicni,
        PrijemceObec = dotaceRec.UcastnikNazevObceDorucPosta,
        PrijemceOkres = dotaceRec.UcastnikNazevOkresu,
        PrijemcePSC = dotaceRec.UcastnikPsc,
        PrijemceUlice = dotaceRec.UcastnikUliceCPCECO,
        PrijemceRokNarozeni = Steps.ConvertRodneCisloToDate(dotaceRec.UcastnikRcUcastnika)?.Year,
        ZdrojNazev = "dotinfo",
        ZdrojUrl = dotaceRec.Url,
        Rozhodnuti = rozhodnuti
    };
    dotaceResults.Add(dotace);
}

appLogger.Debug("Uploading dotace to db");

await DotaceRepo.SaveDotaceToDb(dotaceResults, appLogger, dbIntermediateCnnString);
appLogger.Debug("Finished");