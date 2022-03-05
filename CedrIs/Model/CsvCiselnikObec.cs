using CsvHelper.Configuration.Attributes;

namespace CedrIs.Model;

public class CsvCiselnikObec
{
    public string Id { get; set; }
    public string ObecNazev { get; set; }
    public string OkresNad { get; set; }
    
    [Ignore]
    public string Okres { get; set; }
}