using System;
using System.Collections.Generic;

namespace SZIFParser
{
    class SZIFobj
    {
        public string IdDotace { get; set; }
        public DateTime? PodpisDatum { get; set; }
        public Prijemce Prijemce { get; set; }
        public string ZdrojUrl { get; set; }
        public string ZdrojNazev { get; set; }
        public string IdProjektu { get; set; }
        public string NazevProjektu { get; set; }
        public string KodProjektu { get; set; }
        public float DotaceCelkem { get; set; }
        public float PujckaCelkem { get; set; }
        public List<Rozhodnuti> Rozhodnuti { get; set; }
        public DotacniProgram Program { get; set; }
    }
}
