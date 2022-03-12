using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Eufondy.Model
{
    [Keyless]
    [Table("dotace2006", Schema = "eufondy")]
    public partial class Dotace2006
    {
        [Column("kod_projektu")]
        public string? KodProjektu { get; set; }
        [Column("cislo_programu")]
        public string? CisloProgramu { get; set; }
        [Column("nazev_programu")]
        public string? NazevProgramu { get; set; }
        [Column("cislo_priority")]
        public string? CisloPriority { get; set; }
        [Column("nazev_priority")]
        public string? NazevPriority { get; set; }
        [Column("cislo_opatreni")]
        public string? CisloOpatreni { get; set; }
        [Column("nazev_opatreni")]
        public string? NazevOpatreni { get; set; }
        [Column("nazev_projektu")]
        public string? NazevProjektu { get; set; }
        [Column("stav_projektu")]
        public string? StavProjektu { get; set; }
        [Column("zadatel")]
        public string? Zadatel { get; set; }
        [Column("ulice")]
        public string? Ulice { get; set; }
        [Column("cislo_popisne")]
        public double? CisloPopisne { get; set; }
        [Column("cislo_orientacni")]
        public string? CisloOrientacni { get; set; }
        [Column("obec")]
        public string? Obec { get; set; }
        [Column("psc")]
        public string? Psc { get; set; }
        [Column("smlouva__eu_podil")]
        public double? SmlouvaEuPodil { get; set; }
        [Column("smlouva_narodni_verejne_prostredky")]
        public double? SmlouvaNarodniVerejneProstredky { get; set; }
        [Column("proplaceno_eu_podil")]
        public double? ProplacenoEuPodil { get; set; }
        [Column("proplaceno_narodni_verejne_prostredky")]
        public double? ProplacenoNarodniVerejneProstredky { get; set; }
        [Column("zahajeni_projektu", TypeName = "timestamp without time zone")]
        public DateTime? ZahajeniProjektu { get; set; }
        [Column("ukonceni_projektu", TypeName = "timestamp without time zone")]
        public DateTime? UkonceniProjektu { get; set; }
        [Column("code_town")]
        public double? CodeTown { get; set; }
        [Column("code_district")]
        public string? CodeDistrict { get; set; }
        [Column("code_count")]
        public string? CodeCount { get; set; }
        [Column("kod_nuts")]
        public string? KodNuts { get; set; }
        [Column("nazev_nuts")]
        public string? NazevNuts { get; set; }
    }
}
