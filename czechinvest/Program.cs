﻿using System.Text.Json;
using Common;
using Common.IntermediateDb;
using CzechInvest.Model;
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
await using var db = new CzechInvestDbContext(dotInfoCnnString);
var szif = await db.CzechInvest
    .Where(d => d.Zruseno == null)
    .ToListAsync();

foreach (var dotaceRec in szif)
{
    
    List<Rozhodnuti> rozhodnuti = new()
    {
        new Rozhodnuti()
        {
            Rok = (int?)dotaceRec.RozhodnutiRok,
            CastkaRozhodnuta = (decimal)dotaceRec.RozhodnutiMilCzk * 1_000_000m,
            ZdrojFinanci = "CZ",
        }
        
    };
    
    Dotace dotace = new()
    {
        Id = $"czechinvest-{dotaceRec.Id?.ToString()}",
        IdDotace = dotaceRec.Id?.ToString(),
        DatumPodpisu = dotaceRec.RokPodani is null ? null : new DateTime((int)dotaceRec.RokPodani, 1, 1), 
        ProgramNazev = dotaceRec.Program,
        NazevProjektu = dotaceRec.Projekt,
        PrijemceObchodniJmeno = dotaceRec.Prijemce,
        PrijemceIco = dotaceRec.Ico?.Replace(" ",""),
        
        ZdrojNazev = "czechinvest",
        ZdrojUrl = "https://www.czechinvest.org/cz/Sluzby-pro-investory/Investicni-pobidky",
        Rozhodnuti = rozhodnuti
    };
    dotaceResults.Add(dotace);
}

appLogger.Debug("Uploading dotace to db");

await DotaceRepo.SaveDotaceToDb(dotaceResults, appLogger, dbIntermediateCnnString);
appLogger.Debug("Finished");