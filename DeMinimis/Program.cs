using System;
using System.Collections.Generic;
using System.IO.Compression;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using Polly;
using Polly.Extensions.Http;
using Polly.Retry;

namespace DeMinimis
{
    class Program
    {
        private static int _maxConcurrency;
        private static SemaphoreSlim _semaphore = new(10);
        
        private static readonly JsonSerializerOptions Options = new()
        {
            PropertyNameCaseInsensitive = true
        };
        
        private static readonly AsyncRetryPolicy<HttpResponseMessage> HttpPolicy = HttpPolicyExtensions
            .HandleTransientHttpError()
            .WaitAndRetryAsync(new[]
            {
                TimeSpan.FromSeconds(1),
                TimeSpan.FromSeconds(2),
                TimeSpan.FromSeconds(3)
            });

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

            _maxConcurrency = maxConcurrency;
            _semaphore = new SemaphoreSlim(_maxConcurrency);
            var dumpUri = new Uri(dumpUrl);
            using var httpClient = new HttpClient();
            httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Token", hlidacToken);
            var deMinimisRepository = new DeMinimisRepository(connectionString);
            Console.WriteLine("Init done");
            
            await deMinimisRepository.CreateTable();
            Console.WriteLine("DB table created.");

            // download dump with IDs from Hlidac
            var recordIds = await LoadHlidacIds(httpClient, dumpUri, HttpPolicy);
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
                var deMinimisTask = GetResult<DeMinimisDetail>(HttpPolicy, httpClient, uri);
                taskList.Add(deMinimisTask);
            }

            var results = await Task.WhenAll(taskList);
            Console.WriteLine("All records downloaded.");
            
            // todo: save data
            await deMinimisRepository.InsertMany(results);
            Console.WriteLine("All records inserted.");
        }
        

        private static async Task<T?> GetResult<T>(AsyncRetryPolicy<HttpResponseMessage> policy,
            HttpClient httpClient, Uri uri)
        {
            try
            {
                await _semaphore.WaitAsync();

                var response = await policy.ExecuteAsync(async () => await httpClient.GetAsync(uri));
                if (response.IsSuccessStatusCode)
                {
                    var stream = await response.Content.ReadAsStreamAsync();
                    var result = await JsonSerializer.DeserializeAsync<T>(stream, Options);
                    return result;
                }
                else
                {
                    Console.WriteLine($"Error response [{response.StatusCode}] when requesting {uri}");
                }
            }
            catch (Exception ex)
            {
                
                Console.WriteLine($"Error occured - [{ex.Message}]");
            }
            finally
            {
                _semaphore.Release();
            }

            return default;
        }

        private static async Task<List<HlidacDump>?> LoadHlidacIds(HttpClient httpClient, Uri url,
            AsyncRetryPolicy<HttpResponseMessage> policy)
        {
            var response = await policy.ExecuteAsync(async () => await httpClient.GetAsync(url));
            if (response.IsSuccessStatusCode)
            {
                var responseStream = await response.Content.ReadAsStreamAsync();
                using var zipArchive = new ZipArchive(responseStream, ZipArchiveMode.Read);
                var zipArchiveEntry = zipArchive.Entries.First();
                await using var openedEntry = zipArchiveEntry.Open();
                
                var ids = await JsonSerializer.DeserializeAsync<List<HlidacDump>>(openedEntry, Options);
                return ids;
            }
            else
            {
                Console.WriteLine($"Error response [{response.StatusCode}] when requesting {url}");
            }

            return default;
        }
    }
}
