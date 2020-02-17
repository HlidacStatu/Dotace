using System;

namespace SZIFParser
{
    public class Rozhodnuti
    {
        public string Id { get; set; }
        public decimal CastkaPozadovana { get; set; }
        public decimal CastkaRozhodnuta { get; set; }
        public bool JePujcka { get; set; }
        public string PoskytovatelIco { get; set; }
        public string PoskytovatelNazev { get; set; }
        public string ZdrojFinanci { get; set; }
        public DateTime? Datum { get; set; }
    }
}
