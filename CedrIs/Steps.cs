using System.Text;
using Common;

namespace CedrIs;

public static class Steps
{
    //first step is to download source files
    // public static async Task DownloadFiles(string folder, HttpWrapper clientWrapper)
    // {
    //     string graphQlQuery =
    //         "{\"query\":\"query { datasets ( limit: 100 filters: { isPartOf: \\\"https://data.gov.cz/zdroj/datové-sady/00006947/a77f25e1dfee2b3ec4be09f02c5fc197\\\" }) {  data { distribution { accessURL } } pagination { totalCount } } }\"}";
    //     var httpContent = new StringContent(graphQlQuery, Encoding.UTF8, "application/json");
    //     var dataGovResults =
    //         await clientWrapper.PostResult<DataGovResult>(new Uri("https://data.gov.cz/graphql"), httpContent);
    //     var sourceUrls = dataGovResults.Data.Datasets.Data
    //         .SelectMany(x => x.Distribution.Select(y => y.AccessURL))
    //         .Where(u => u.Contains(".csv.gz"));
    //
    //
    //     var tasks = new List<Task>();
    //     foreach (var sourceUrl in sourceUrls)
    //     {
    //         tasks.Add(clientWrapper.DownloadFile(new Uri(sourceUrl), folder));
    //     }
    //
    //     Task.WaitAll(tasks.ToArray());
    // }
    
    public static async Task DownloadFiles(string folder, HttpWrapper clientWrapper)
    {

        var sourceUrls = await File.ReadAllLinesAsync(@"downloadList.csv");

        var tasks = new List<Task>();
        foreach (var sourceUrl in sourceUrls)
        {
            tasks.Add(clientWrapper.DownloadFile(new Uri(sourceUrl), folder));
        }

        Task.WaitAll(tasks.ToArray());
    }
}