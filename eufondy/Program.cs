using System.Text.Json;
using Common;
using Common.IntermediateDb;
using Eufondy;
using Eufondy.Model;
using Microsoft.EntityFrameworkCore;

var appLogger = Logging.CreateLogger("application.log");

appLogger.Debug("Start Eufondy!");
appLogger.Debug("Loading configuration...");

var configFile = File.OpenRead("appsettings.json");
var config = await JsonSerializer.DeserializeAsync<Dictionary<string, string>>(configFile);
if (config == null || config.Count == 0) throw new ArgumentNullException(nameof(config));
var dbIntermediateCnnString = config["dbintermediate"];
var eufondyCnnString = config["dbeufondy"];


var dotaceResults = new List<Dotace>();

appLogger.Debug("Adding 2006 set");
string setprep06 = "04-06-";
List<Dotace2006>? dotace2006;
await using (var eufondyDb = new EufondyDbContext(eufondyCnnString))
{
    dotace2006 = await eufondyDb.Dotace06
        .FromSqlRaw(@"select distinct on (kod_projektu) * from eufondy.dotace2006") // je potřeba filtrovat duplicity
        .ToListAsync();
}

foreach (var dotaceRec in dotace2006)
{
    var rozhodnuti = Steps.CreateRozhodnuti(dotaceRec.SmlouvaNarodniVerejneProstredky,
        dotaceRec.ProplacenoNarodniVerejneProstredky,
        dotaceRec.SmlouvaEuPodil,
        dotaceRec.ProplacenoEuPodil);

    var dotace = new Dotace()
    {
        Id = $"eufondy-{setprep06}{dotaceRec.KodProjektu}",
        IdDotace = $"{setprep06}{dotaceRec.KodProjektu}",
        ProgramKod = dotaceRec.CisloProgramu,
        ProgramNazev = dotaceRec.NazevProgramu,
        PrijemceObchodniJmeno = dotaceRec.Zadatel,
        PrijemceObec = dotaceRec.Obec,
        PrijemcePSC = dotaceRec.Psc,
        PrijemceUlice =
            $"{dotaceRec.Ulice} {dotaceRec.CisloPopisne?.ToString() ?? dotaceRec.CisloOrientacni?.ToString()}".Trim(),
        KodProjektu = dotaceRec.KodProjektu,
        NazevProjektu = dotaceRec.NazevProjektu,
        DatumPodpisu = dotaceRec.ZahajeniProjektu,
        ZdrojUrl = "https://www.dotaceeu.cz/cs/evropske-fondy-v-cr/programove-obdobi-2004-2006-(1)/vysledky-cerpani",
        ZdrojNazev = "eufondy 2004-2006",
        Rozhodnuti = rozhodnuti,
    };
    dotaceResults.Add(dotace);
}
dotace2006 = null;

appLogger.Debug("Adding 2013 set");
string setprep13 = "07-13-";
List<Dotace2013>? dotace2013;
await using (var eufondyDb = new EufondyDbContext(eufondyCnnString))
{
    dotace2013 = await eufondyDb.Dotace13
        .ToListAsync();
}

foreach (var dotaceRec in dotace2013)
{
    
    var rozhodnutoCr = dotaceRec.RozhodnutiSmlouvaOPoskytnutiDotaceVerejneProstredkyCelke -
                       dotaceRec.RozhodnutiSmlouvaOPoskytnutiDotaceEuZdroje;
    var cerpanoCr = dotaceRec.ProplaceneProstredkyPrijemcumVyuctovaneVerejneProstredkyC -
                    dotaceRec.ProplaceneProstredkyPrijemcumVyuctovaneEuZdroje;
    var rozhodnutoEu = dotaceRec.RozhodnutiSmlouvaOPoskytnutiDotaceEuZdroje;
    var cerpanoEu = dotaceRec.ProplaceneProstredkyPrijemcumVyuctovaneEuZdroje;

    var rozhodnuti = Steps.CreateRozhodnuti(rozhodnutoCr, cerpanoCr, rozhodnutoEu, cerpanoEu);

    var programSplit = dotaceRec.CisloANazevProgramu?.Split(" ", 2, StringSplitOptions.TrimEntries);
    var kodProgramu = programSplit?.Length > 0 ? programSplit[0] : null;
    var nazevProgramu = programSplit?.Length > 1 ? programSplit[1] : null;
    
    var adresaSplit = dotaceRec.AdresaZadatele?.Split(",", 2, StringSplitOptions.TrimEntries);
    var psc = adresaSplit?.Length > 0 ? Steps.GetPscFromAddress(adresaSplit[0]) : null;
    var ulice = adresaSplit?.Length > 0 ? adresaSplit[1].Replace("/", "") : null;
    
    
    var dotace = new Dotace()
    {
        Id = $"eufondy-{setprep13}{dotaceRec.KodProjektu}",
        IdDotace = $"{setprep13}{dotaceRec.KodProjektu}",
        ProgramKod = kodProgramu,
        ProgramNazev = nazevProgramu,
        PrijemceIco = dotaceRec.IcZadatele,
        PrijemceObchodniJmeno = dotaceRec.Zadatel,
        PrijemceObec = dotaceRec.ObecZadateleNazev,
        PrijemcePSC = psc,
        PrijemceUlice = ulice,
        KodProjektu = dotaceRec.KodProjektu,
        NazevProjektu = dotaceRec.NazevProjektu,
        DatumPodpisu = dotaceRec.DatumPodpisuSmlouvyRozhodnuti,
        ZdrojUrl = "https://dotaceeu.cz/cs/evropske-fondy-v-cr/programove-obdobi-2007-2013/cerpani-v-obdobi-2007-2013",
        ZdrojNazev = "eufondy 2007-2013",
        Rozhodnuti = rozhodnuti,
    };
    dotaceResults.Add(dotace);
}
dotace2013 = null;


appLogger.Debug("Adding 2020 set");
string setprep20 = "14-20-";
List<Dotace2020>? dotace2020;
await using (var eufondyDb = new EufondyDbContext(eufondyCnnString))
{
    dotace2020 = await eufondyDb.Dotace20
        .ToListAsync();
}

foreach (var dotaceRec in dotace2020)
{
    int? rokRozhodnuti = null;
    if (int.TryParse(dotaceRec.DatumZahajeni?.Split("-")?.FirstOrDefault(), out var rok))
    {
        rokRozhodnuti = rok;
    }
    
    var rozhodnuti = new List<Rozhodnuti>()
    {
        new Rozhodnuti()
        {
            Cerpani = new List<Cerpani>(),
            Poskytovatel = "ESIF",
            Rok = rokRozhodnuti, 
            CastkaRozhodnuta = Convert.ToDecimal(Math.Round(dotaceRec.FinancovaniCzv ?? 0, 2))
        }
    };

    
    var dotace = new Dotace()
    {
        Id = $"eufondy-{setprep20}{dotaceRec.KodProjektu}",
        IdDotace = $"{setprep20}{dotaceRec.KodProjektu}",
        PrijemceIco = dotaceRec.ZadatelIco?.ToString("N0"),
        PrijemceObchodniJmeno = dotaceRec.ZadatelNazev,
        PrijemceObec = dotaceRec.ZadatelObec,
        PrijemcePSC = dotaceRec.ZadatelPsc,
        PrijemceOkres = dotaceRec.ZadatelOkres,
        KodProjektu = dotaceRec.KodProjektu,
        NazevProjektu = dotaceRec.Naz,
        DatumPodpisu = dotaceRec.DatumZahajeni != null ? DateTime.Parse(dotaceRec.DatumZahajeni) : null,
        ZdrojUrl = "https://ms14opendata.mssf.cz/SeznamProjektu.xml",
        ZdrojNazev = "eufondy 2014-2020",
        Rozhodnuti = rozhodnuti,
    };
    dotaceResults.Add(dotace);
}
dotace2020 = null;

appLogger.Debug("Uploading dotace to db");

await DotaceRepo.SaveDotaceToDb(dotaceResults, appLogger, dbIntermediateCnnString);
appLogger.Debug("Finished");