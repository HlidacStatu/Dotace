using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text.Json;
using System.Threading.Tasks;
using Common;
using Common.IntermediateDb;

namespace DeMinimis
{
    class Program
    {

        /// <summary>
        /// 
        /// </summary>
        /// <param name="connectionString">Db connection string in format "Host=localhost;Database=import;Username=dialogs;Password=dialogs;SearchPath=deminimis"</param>
        /// <param name="hlidacToken">Token obtained from "https://www.hlidacstatu.cz/api/v1/Index"</param>
        /// <param name="maxConcurrency">How many requests are sent to server (on parallel)</param>
        /// <param name="dumpUrl">Address where dump file with ids can be found</param>
        /// <returns></returns>
        /// <exception cref="ArgumentException"></exception>
        public static async Task Main(string hlidacToken,
            int maxConcurrency = 10,
            string dumpUrl = "https://www.hlidacstatu.cz/api/v1/dump?datatype=dataset.de-minimis")
        {
       
            if (string.IsNullOrWhiteSpace(hlidacToken))
                throw new ArgumentException("Value cannot be null or whitespace.", nameof(hlidacToken));
            
            
            var appLogger = Logging.CreateLogger("application.log");

            appLogger.Debug("Start Dotinfo!");
            appLogger.Debug("Loading configuration...");

            string cnnString = DataHelper.GetDbConnectionString();

            appLogger.Debug("Prepare dbs");

            //intermediate db
            await using (var intermediateDbContext = new IntermediateDbContext(cnnString))
            {
                await intermediateDbContext.Database.EnsureCreatedAsync();
            }

            appLogger.Debug($"api key=[{hlidacToken}]");
            appLogger.Debug("Starting");

            
            var dumpUri = new Uri(dumpUrl);
            using var httpClient = new HttpClient();
            httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Token", hlidacToken);
            var clientWrapper = new HttpWrapper(httpClient, maxConcurrency);
            
            appLogger.Debug("Init done");
            
            // download dump with IDs from Hlidac
            var recordIds = await clientWrapper.GetZippedResult<List<HlidacDump>?>(dumpUri);
            if (recordIds is null)
            {
                throw new Exception($"No data were loaded from {dumpUri}");
            }
            appLogger.Debug("Dump downloaded.");

            var baseRecordAddress = "https://www.hlidacstatu.cz/api/v2/datasety/de-minimis/zaznamy/";
            
            List<Task<DeMinimisDetail?>> taskList = new();
            foreach (var recordId in recordIds)
            {
                var uri = new Uri(baseRecordAddress + recordId.Id);
                var deMinimisTask = clientWrapper.GetResult<DeMinimisDetail>(uri);
                taskList.Add(deMinimisTask);
            }

            var results = await Task.WhenAll(taskList);
            appLogger.Debug("All records downloaded.");

            
            var dotaceResults = new List<Dotace>();
            foreach (var deMinimisDetail in results)
            {
                if (deMinimisDetail is null)
                    continue;
                
                var nechteneDotace = deMinimisDetail.StavKod == 4
                                     || deMinimisDetail.PodporaForma_Kod == 61;
                if(nechteneDotace)
                    continue;

                
                var id = deMinimisDetail.Id.ToString();
                
                List<Rozhodnuti> rozhodnuti = new()
                {
                    new Rozhodnuti()
                    {
                        Rok = deMinimisDetail.PodporaDatum.Year,
                        CastkaRozhodnuta = deMinimisDetail.PodporaCzk,
                        Poskytovatel = deMinimisDetail.PoskytovatelOjm,
                        IcoPoskytovatele = deMinimisDetail.PoskytovatelIco,
                        JePujcka = deMinimisDetail.PodporaForma_Kod == 69 
                                   || deMinimisDetail.PodporaForma_Kod == 50
                                   || deMinimisDetail.PodporaForma_Kod == 51,
                        Cerpani = new List<Cerpani>()
                        {
                            new()
                            {
                                CastkaSpotrebovana = deMinimisDetail.PodporaCzk,
                                Rok = deMinimisDetail.PodporaDatum.Year
                            }
                        }
                                   
                        
                  
                    }
                };
                
                Dotace dotace = new()
                {
                    Id = $"deminimis-{id}",
                    IdDotace = id,
                    KodProjektu = deMinimisDetail.ProjektId,
                    NazevProjektu = deMinimisDetail.PodporaUcel,
                    DatumAktualizace = deMinimisDetail.Edidat,
                    DatumPodpisu = deMinimisDetail.PodporaDatum,
                    PrijemceObchodniJmeno = deMinimisDetail.JmenoPrijemce,
                    PrijemceIco = deMinimisDetail.Ico,
                    ZdrojNazev = "deminimis",
                    ZdrojUrl = $"https://www.hlidacstatu.cz/data/Detail/de-minimis/{id}",
                    Rozhodnuti = rozhodnuti
                };
                dotaceResults.Add(dotace);
            }

            
            appLogger.Debug("Uploading dotace to db");

            await DotaceRepo.SaveDotaceToDb(dotaceResults, appLogger, cnnString);
            appLogger.Debug("Finished");
        }
    }
}
