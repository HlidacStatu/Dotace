﻿using System.Text.Json;
using System.Text.RegularExpressions;
using Common;
using Common.IntermediateDb;
using Microsoft.EntityFrameworkCore;
using Szif.Model;

var appLogger = Logging.CreateLogger("application.log");

appLogger.Debug("Start Eufondy!");
appLogger.Debug("Loading configuration...");

string cnnString = DataHelper.GetDbConnectionString();

appLogger.Debug("Prepare dbs");

//intermediate db
await IntermediateDbContext.EnsureDbIsCreated(cnnString);


var dotaceResults = new List<Dotace>();

await using var db = new SzifDbContext(cnnString);
var szif = await db.Szif.ToListAsync();

foreach (var dotaceRec in szif)
{
    int? rok = null;
    var reMatch = Regex.Match(dotaceRec.Id, @"(\d{4})-");
    if (reMatch.Success)
    {
        rok = int.Parse(reMatch.Groups[1].Value);
    }

    var castkaCr = DataHelper.GetPriceFromText(dotaceRec.CastkaCr);
    var castkaEu = DataHelper.GetPriceFromText(dotaceRec.CastkaEu);
    
    List<Rozhodnuti> rozhodnuti = new()
    {
        new Rozhodnuti()
        {
            CastkaRozhodnuta = 0,
            Rok = rok,
            Poskytovatel = "CZ",
            ZdrojFinanci = dotaceRec.Zdroj,
            Cerpani = new()
            {
                new Cerpani()
                {
                    CastkaSpotrebovana = castkaCr
                }
            
            }
        },
        new Rozhodnuti()
        {
            CastkaRozhodnuta = 0,
            Rok = rok,
            Poskytovatel = "EU",
            ZdrojFinanci = dotaceRec.Zdroj,
            Cerpani = new()
            {
                new Cerpani()
                {
                    CastkaSpotrebovana = castkaEu
                }
            
            }
        }
        
    };
    
    Dotace dotace = new()
    {
        Id = $"szif-{dotaceRec.Id}",
        IdDotace = dotaceRec.Id,
        NazevProjektu = dotaceRec.Opatreni,
        ProgramNazev = dotaceRec.Opatreni,
        PrijemceObchodniJmeno = dotaceRec.Jmeno,
        PrijemceIco = dotaceRec.Ico?.ToString("F0"),
        PrijemceObec = dotaceRec.Obec,
        PrijemceOkres = dotaceRec.Okres,
        
        ZdrojNazev = "szif",
        ZdrojUrl = "https://www.szif.cz/cs",
        Rozhodnuti = rozhodnuti
    };
    dotaceResults.Add(dotace);
}

appLogger.Debug("Uploading dotace to db");

await DotaceRepo.SaveDotaceToDb(dotaceResults, appLogger, cnnString);
appLogger.Debug("Finished");