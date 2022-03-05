using System.Globalization;
using System.IO.Compression;
using System.Text;
using CsvHelper;
using CsvHelper.Configuration;
using Serilog;

namespace Common;

public static class CsvHelpers
{
    private static ILogger _logger = Logging.CreateLogger("csv.log");
    
    public static async Task<Dictionary<string, T>> LoadCsvAsDictionary<T>(string folder,
        Func<T, string> keyselector,
        string? filename = null) where T : class
    {
        string path = SetupFilePath<T>(filename, folder);

        await using var reader = File.OpenRead(path);
        await using var gzipStream = new GZipStream(reader, CompressionMode.Decompress);
        using var streamReader = new StreamReader(gzipStream, Encoding.UTF8);

        var csvConfig = new CsvConfiguration(CultureInfo.InvariantCulture)
        {
            PrepareHeaderForMatch = header => header.Header.Trim().ToLower(),
            BadDataFound = (context) =>
            {
                _logger.Error($"{path} contains corrupted data at {context.RawRecord}");
                
            },
            MissingFieldFound = (context) =>
            {
                _logger.Debug($"{path} is missing fields {string.Join(',', context.HeaderNames)} at {context.Index}");
            },
            HeaderValidated = (context) =>
            {
                _logger.Debug($"{path} is missing header {string.Join(',', context.InvalidHeaders.SelectMany(h => h.Names))}");
            }
        };

        using var csv = new CsvReader(streamReader, csvConfig);
        var records = csv.GetRecords<T>();
        return records.ToDictionary(keyselector);
    }


    public static async Task<Dictionary<string, List<T>>> LoadCsvAsDictionaryList<T>(string folder,
        Func<T, string> keyselector,
        string? filename = null) where T : class
    {
        string path = SetupFilePath<T>(filename, folder);
        await using var reader = File.OpenRead(path);
        await using var gzipStream = new GZipStream(reader, CompressionMode.Decompress);
        using var streamReader = new StreamReader(gzipStream, Encoding.UTF8);

        var csvConfig = new CsvConfiguration(CultureInfo.InvariantCulture)
        {
            PrepareHeaderForMatch = header => header.Header.Trim().ToLower(),
            BadDataFound = (context) =>
            {
                _logger.Error($"{path} contains corrupted data at {context.RawRecord}");
                
            },
            MissingFieldFound = (context) =>
            {
                _logger.Debug($"{path} is missing fields {string.Join(',',context.HeaderNames)} at {context.Index}");
            },
            HeaderValidated = (context) =>
            {
                _logger.Debug($"{path} is missing header {string.Join(',', context.InvalidHeaders.SelectMany(h => h.Names))}");
            }
        };

        using var csv = new CsvReader(streamReader, csvConfig);
        var records = csv.GetRecords<T>();

        var result = new Dictionary<string, List<T>>();
        foreach (var record in records)
        {
            var key = keyselector(record);

            if (!result.ContainsKey(key))
            {
                result.Add(key, new List<T>());
            }

            result[key].Add(record);
        }

        return result;
    }

    private static string SetupFilePath<T>(string? filename, string folder) where T : class
    {
        if (filename == null)
        {
            var modelName = typeof(T).Name.ToLower().Replace("csv", "");
            filename = $"{modelName}.csv.gz";
        }

        string path = Path.Combine(folder, filename);

        return path;
    }
}