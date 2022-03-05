using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using Common;

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
        public static async Task Main(string connectionString,
            string hlidacToken,
            int maxConcurrency = 10,
            string dumpUrl = "https://www.hlidacstatu.cz/api/v1/dump?datatype=dataset.de-minimis")
        {
            if (string.IsNullOrWhiteSpace(connectionString))
                throw new ArgumentException("Value cannot be null or whitespace.", nameof(connectionString));
            if (string.IsNullOrWhiteSpace(hlidacToken))
                throw new ArgumentException("Value cannot be null or whitespace.", nameof(hlidacToken));
            
            Console.WriteLine($"cnn string=[{connectionString}]");
            Console.WriteLine($"api key=[{hlidacToken}]");
            Console.WriteLine("Starting");

            
            var dumpUri = new Uri(dumpUrl);
            using var httpClient = new HttpClient();
            httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Token", hlidacToken);
            var clientWrapper = new HttpWrapper(httpClient, maxConcurrency);
            var deMinimisRepository = new DeMinimisRepository(connectionString);
            Console.WriteLine("Init done");
            
            await deMinimisRepository.CreateTable();
            Console.WriteLine("DB table created.");

            // download dump with IDs from Hlidac
            var recordIds = await clientWrapper.GetZippedResult<List<HlidacDump>?>(dumpUri);
            if (recordIds is null)
            {
                throw new Exception($"No data were loaded from {dumpUri}");
            }
            Console.WriteLine("Dump downloaded.");

            var baseRecordAddress = "https://www.hlidacstatu.cz/api/v2/datasety/de-minimis/zaznamy/";
            
            List<Task<DeMinimisDetail?>> taskList = new();
            foreach (var recordId in recordIds)
            {
                var uri = new Uri(baseRecordAddress + recordId.Id);
                var deMinimisTask = clientWrapper.GetResult<DeMinimisDetail>(uri);
                taskList.Add(deMinimisTask);
            }

            var results = await Task.WhenAll(taskList);
            Console.WriteLine("All records downloaded.");
            
            // todo: save data
            await deMinimisRepository.InsertMany(results);
            Console.WriteLine("All records inserted.");
        }
    }
}
