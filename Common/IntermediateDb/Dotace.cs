using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Common.IntermediateDb
{
    public class Dotace
    {
        [Key]
        public string Id { get; set; }
        
        public string IdDotace { get; set; }
        public DateTime? DatumPodpisu { get; set; }
        public DateTime? DatumAktualizace { get; set; }
        public string? KodProjektu { get; set; }
        public string? NazevProjektu { get; set; }
        public string? Duplicita { get; set; }
        
        [Column(TypeName = "jsonb")]
        public List<Rozhodnuti> Rozhodnuti { get; set; } = new List<Rozhodnuti>();
        
        [Column(TypeName = "jsonb")]
        public List<string>? Chyba { get; set; }
        
        public string? ProgramNazev { get; set; }
        public string? ProgramKod { get; set; }
        
        public string? PrijemceIco { get; set; }
        public string? PrijemceObchodniJmeno { get; set; }
        public string? PrijemceHlidacJmeno { get; set; }
        public string? PrijemceJmeno { get; set; }
        public int? PrijemceRokNarozeni { get; set; }
        public string? PrijemceObec { get; set; }
        public string? PrijemceOkres { get; set; }
        public string? PrijemcePSC { get; set; }
        public string? PrijemceUlice { get; set; }
        public string ZdrojNazev { get; set; }
        public string ZdrojUrl { get; set; }

    }
}
