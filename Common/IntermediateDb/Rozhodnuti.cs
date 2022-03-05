using System.ComponentModel.DataAnnotations.Schema;

namespace Common.IntermediateDb
{
    public class Rozhodnuti
    {
        public string Id { get; set; }
        public decimal? CastkaPozadovana { get; set; }
        public decimal? CastkaRozhodnuta { get; set; }
        public bool? JePujcka { get; set; }
        public string IcoPoskytovatele { get; set; }
        public string Poskytovatel { get; set; }
        public int? Rok { get; set; }
        public string ZdrojFinanci { get; set; }
        public List<Cerpani> Cerpani { get; set; } = new List<Cerpani>();

    }
}
