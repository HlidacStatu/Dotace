using System.IO.Compression;
using System.Net.Http.Headers;
using System.Text.Json;
using Polly;
using Polly.Extensions.Http;
using Polly.Retry;

namespace Common;

public class HttpWrapper
{
    private SemaphoreSlim _semaphore;
    private HttpClient _httpClient;

    private readonly JsonSerializerOptions _options = new()
    {
        PropertyNameCaseInsensitive = true
    };

    private readonly AsyncRetryPolicy<HttpResponseMessage> _httpPolicy = HttpPolicyExtensions
        .HandleTransientHttpError()
        .WaitAndRetryAsync(new[]
        {
            TimeSpan.FromSeconds(1),
            TimeSpan.FromSeconds(2),
            TimeSpan.FromSeconds(3)
        });

    public HttpWrapper(HttpClient httpClient, int maxConcurrency = 10)
    {
        _httpClient = httpClient;
        maxConcurrency = maxConcurrency < 1 ? 1 : maxConcurrency;
        _semaphore = new SemaphoreSlim(maxConcurrency);
    }

    public async Task<T?> GetResult<T>(Uri uri)
    {
        try
        {
            await _semaphore.WaitAsync();

            using var response = await _httpPolicy.ExecuteAsync(async () => await _httpClient.GetAsync(uri));
            if (response.IsSuccessStatusCode)
            {
                await using var stream = await response.Content.ReadAsStreamAsync();
                var result = await JsonSerializer.DeserializeAsync<T>(stream, _options);
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

    public async Task<T?> GetZippedResult<T>(Uri url)
    {
        try
        {
            await _semaphore.WaitAsync();

            using var response = await _httpPolicy.ExecuteAsync(async () => await _httpClient.GetAsync(url));
            if (response.IsSuccessStatusCode)
            {
                await using var responseStream = await response.Content.ReadAsStreamAsync();
                using var zipArchive = new ZipArchive(responseStream, ZipArchiveMode.Read);
                var zipArchiveEntry = zipArchive.Entries.First();
                await using var openedEntry = zipArchiveEntry.Open();

                var ids = await JsonSerializer.DeserializeAsync<T>(openedEntry, _options);
                return ids;
            }
            else
            {
                Console.WriteLine($"Error response [{response.StatusCode}] when requesting {url}");
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
    
    public async Task<T?> PostResult<T>(Uri uri, HttpContent httpContent)
    {
        try
        {
            await _semaphore.WaitAsync();
            
            using var response = await _httpPolicy.ExecuteAsync(async () => await _httpClient.PostAsync(uri, httpContent));
            if (response.IsSuccessStatusCode)
            {
                await using var stream = await response.Content.ReadAsStreamAsync();
                var result = await JsonSerializer.DeserializeAsync<T>(stream, _options);
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

    public async Task DownloadFile(Uri uri, string folderPath)
    {
        try
        {
            await _semaphore.WaitAsync();

            using var request = new HttpRequestMessage(HttpMethod.Get, uri);
            request.Headers.Add("Accept-Encoding", "gzip");
            request.Headers.Add("Accept", "text/html,application/xhtml+xml,application/xml,application/json;q=0.9,image/avif,image/webp,image/apng,*/*;q=0.8,application/signed-exchange;v=b3;q=0.9");
            request.Headers.CacheControl = new CacheControlHeaderValue { NoCache = true };

            //using var response = await _httpPolicy.ExecuteAsync(async () => await _httpClient.GetAsync(uri));
            using var response = await _httpPolicy.ExecuteAsync(async () => await _httpClient.SendAsync(request));
            if (response.IsSuccessStatusCode)
            {
                await using var stream = await response.Content.ReadAsStreamAsync();

                //generate file name
                var filename = uri.Segments.LastOrDefault() ?? "file.txt";
                var filepath = Path.Combine(folderPath, filename );
                var filecounter = 0;
                while (File.Exists(filepath))
                {
                    filecounter++;
                    filepath = Path.Combine(folderPath, $"{filecounter}_{filename}");
                }
                
                var fileInfo = new FileInfo(filepath);
                await using var fileStream = fileInfo.OpenWrite();
                await stream.CopyToAsync(fileStream);
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
    }
}