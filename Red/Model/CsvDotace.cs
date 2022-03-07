using System.Globalization;
using CsvHelper.Configuration.Attributes;

namespace Red.Model;

public class CsvDotace
{
    public string Iridotace { get; set; }
    public string Identifikator { get; set; }
    public string Nazev { get; set; }
    
    [DateTimeStyles(DateTimeStyles.AssumeUniversal | DateTimeStyles.AdjustToUniversal)]
    public DateTime? Podpisdatum { get; set; }
    [DateTimeStyles(DateTimeStyles.AssumeUniversal | DateTimeStyles.AdjustToUniversal)]
    public DateTime? Datumaktualizace { get; set; }
    public string Irioperacniprogram { get; set; }
    public string Iriprijemce { get; set; }
}