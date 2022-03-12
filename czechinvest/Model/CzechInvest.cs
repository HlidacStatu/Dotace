using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace CzechInvest.Model
{
    [Keyless]
    [Table("dotace", Schema = "czechinvest")]
    public class CzechInvest
    {
        [Column("id")]
        public long? Id { get; set; }
        [Column("prijemce")]
        public string? Prijemce { get; set; }
        [Column("ico")]
        public string? Ico { get; set; }
        [Column("projekt")]
        public string? Projekt { get; set; }
        [Column("program")]
        public string? Program { get; set; }
        [Column("rozhodnuti_mil_czk")]
        public double? RozhodnutiMilCzk { get; set; }
        [Column("rok_podani")]
        public long? RokPodani { get; set; }
        [Column("rozhodnuti_den")]
        public double? RozhodnutiDen { get; set; }
        [Column("rozhodnuti_mesic")]
        public string? RozhodnutiMesic { get; set; }
        [Column("rozhodnuti_rok")]
        public long? RozhodnutiRok { get; set; }
        [Column("zruseno")]
        public string? Zruseno { get; set; }
    }
}
