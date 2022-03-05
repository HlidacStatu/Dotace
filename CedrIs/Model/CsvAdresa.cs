using CsvHelper.Configuration.Attributes;

namespace CedrIs.Model;

public class CsvAdresa
{
    public string IdPrijemce { get; set; }
    public string Obecnazev { get; set; }
    public string Iriobec { get; set; }
    public string Obec { get; set; }
    public string Psc { get; set; }
    [Ignore]
    public string Okres { get; set; }
}