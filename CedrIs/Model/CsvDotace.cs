using System.Globalization;
using CsvHelper.Configuration.Attributes;

namespace CedrIs.Model;

public class CsvDotace
{
    public string IdDotace { get; set; }
    public string IdPrijemce { get; set; }
    
    public string IriProgram { get; set; }

    public string IriGrantoveSchema { get; set; }
    public string IriOperacniProgram { get; set; }
    public string IriOpatreni { get; set; }


    public string ProjektIdentifikator { get; set; }
    public string ProjektNazev { get; set; }


    [DateTimeStyles(DateTimeStyles.AssumeUniversal | DateTimeStyles.AdjustToUniversal)]
    public DateTime? PodpisDatum { get; set; }

    [DateTimeStyles(DateTimeStyles.AssumeUniversal | DateTimeStyles.AdjustToUniversal)]
    public DateTime? DtAktualizace { get; set; }

}