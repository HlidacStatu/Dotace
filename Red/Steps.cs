using System.Text;
using Common;

namespace Red;

public static class Steps
{
    //first step is to download source files
    public static async Task DownloadFiles(string folder, HttpWrapper clientWrapper)
    {
        string graphQlQuery =
            "{\"query\":\"query { datasets ( limit: 100 filters: { isPartOf: \\\"https://data.gov.cz/zdroj/datové-sady/00006947/4e4762f06ca63a491efc360e793abc09\\\" }) {  data { distribution { accessURL } } pagination { totalCount } } }\"}";
        var httpContent = new StringContent(graphQlQuery, Encoding.UTF8, "application/json");
        var dataGovResults =
            await clientWrapper.PostResult<DataGovResult>(new Uri("https://data.gov.cz/graphql"), httpContent);
        var sourceUrls = dataGovResults.Data.Datasets.Data
            .SelectMany(x => x.Distribution.Select(y => y.AccessURL))
            .Where(u => u.Contains(".csv.gz"));


        var tasks = new List<Task>();
        foreach (var sourceUrl in sourceUrls)
        {
            tasks.Add(clientWrapper.DownloadFile(new Uri(sourceUrl), folder));
        }

        Task.WaitAll(tasks.ToArray());
    }
}