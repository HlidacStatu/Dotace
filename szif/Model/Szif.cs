using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Szif.Model
{
    [Keyless]
    [Table("dotace", Schema = "szif")]
    public class Szif
    {
        [Column("ico")]
        public double? Ico { get; set; }
        [Column("jmeno")]
        public string? Jmeno { get; set; }
        [Column("obec")]
        public string? Obec { get; set; }
        [Column("okres")]
        public string? Okres { get; set; }
        [Column("zdroj")]
        public string? Zdroj { get; set; }
        [Column("opatreni")]
        public string? Opatreni { get; set; }
        [Column("castka_cr")]
        public string? CastkaCr { get; set; }
        [Column("castka_eu")]
        public string? CastkaEu { get; set; }
        [Column("id")]
        public string? Id { get; set; }
    }
}
