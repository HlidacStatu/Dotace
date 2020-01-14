using System;

namespace SZIFParser
{
    public class Rozhodnuti
    {
        public string Id { get; set; }
        public float CastkaPozadovana { get; set; }
        public float CastkaRozhodnuta { get; set; }
        public bool JePujcka { get; set; }
        public string PoskytovatelIco { get; set; }
        public string PoskytovatelNazev { get; set; }
        public string ZdrojFinanci { get; set; }
        public DateTime? Datum { get; set; }
    }
}
