namespace CedrIs.Model;

public class CsvRozhodnuti
{
    public string IdRozhodnuti { get; set; }
    public string IdDotace { get; set; }
    public decimal? CastkaPozadovana { get; set; }
    public decimal? CastkaRozhodnuta { get; set; }
    public int? RokRozhodnuti { get; set; }
    public bool NavratnostIndikator { get; set; }
    
    public string IriPoskytovatelDotace { get; set; }
    public string IriFinancniZdroj { get; set; }
}