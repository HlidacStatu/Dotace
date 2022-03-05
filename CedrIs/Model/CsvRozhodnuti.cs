namespace CedrIs.Model;

public class CsvRozhodnuti
{
    public string Irirozhodnuti { get; set; }
    public int? Rokrozhodnuti { get; set; }
    public decimal? Castkapozadovana { get; set; }
    public decimal? Castkarozhodnuta { get; set; }
    //public bool Investiceindikator { get; set; }
    public bool Navratnostindikator { get; set; }
    //public bool Refundaceindikator { get; set; }
    public string IriDotacePoskytovatel { get; set; }
    public string Irifinancnizdroj { get; set; }
    public string Iridotace { get; set; }
}