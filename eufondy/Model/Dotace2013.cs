using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Eufondy.Model
{
    [Keyless]
    [Table("dotace2013", Schema = "eufondy")]
    public partial class Dotace2013
    {
        [Column("kod_projektu")]
        public string? KodProjektu { get; set; }
        [Column("cislo_a_nazev_programu")]
        public string? CisloANazevProgramu { get; set; }
        [Column("cislo_prioritni_osy")]
        public string? CisloPrioritniOsy { get; set; }
        [Column("cislo_oblasti_podpory")]
        public string? CisloOblastiPodpory { get; set; }
        [Column("nazev_projektu")]
        public string? NazevProjektu { get; set; }
        [Column("popis_projektu")]
        public string? PopisProjektu { get; set; }
        [Column("zadatel")]
        public string? Zadatel { get; set; }
        [Column("ic_zadatele")]
        public string? IcZadatele { get; set; }
        [Column("hosp_pravni_forma")]
        public string? HospPravniForma { get; set; }
        [Column("hosp_pravni_forma_skupina")]
        public string? HospPravniFormaSkupina { get; set; }
        [Column("stav_projektu")]
        public string? StavProjektu { get; set; }
        [Column("datum_podpisu_smlouvy_rozhodnuti", TypeName = "timestamp without time zone")]
        public DateTime? DatumPodpisuSmlouvyRozhodnuti { get; set; }
        [Column("adresa_zadatele")]
        public string? AdresaZadatele { get; set; }
        [Column("kraj_zadatele_kod")]
        public string? KrajZadateleKod { get; set; }
        [Column("kraj_zadatele_nazev")]
        public string? KrajZadateleNazev { get; set; }
        [Column("obec_zadatele_kod")]
        public string? ObecZadateleKod { get; set; }
        [Column("obec_zadatele_nazev")]
        public string? ObecZadateleNazev { get; set; }
        [Column("rozhodnuti_smlouva_o_poskytnuti_dotace_celkove_zdroje_")]
        public double? RozhodnutiSmlouvaOPoskytnutiDotaceCelkoveZdroje { get; set; }
        [Column("rozhodnuti_smlouva_o_poskytnuti_dotace_verejne_prostredky_celke")]
        public double? RozhodnutiSmlouvaOPoskytnutiDotaceVerejneProstredkyCelke { get; set; }
        [Column("rozhodnuti_smlouva_o_poskytnuti_dotace_eu_zdroje_")]
        public double? RozhodnutiSmlouvaOPoskytnutiDotaceEuZdroje { get; set; }
        [Column("proplacene_prostredky_prijemcum_vyuctovane_verejne_prostredky_c")]
        public double? ProplaceneProstredkyPrijemcumVyuctovaneVerejneProstredkyC { get; set; }
        [Column("proplacene_prostredky_prijemcum_vyuctovane_eu_zdroje_")]
        public double? ProplaceneProstredkyPrijemcumVyuctovaneEuZdroje { get; set; }
        [Column("certifikovane_prostredky_verejne_prostredky_celkem_")]
        public double? CertifikovaneProstredkyVerejneProstredkyCelkem { get; set; }
        [Column("certifikovane_prostredky_eu_zdroje_")]
        public double? CertifikovaneProstredkyEuZdroje { get; set; }
        [Column("kod_nuts")]
        public string? KodNuts { get; set; }
        [Column("nazev_nuts")]
        public string? NazevNuts { get; set; }
        [Column("poradi_v_ramci_v_projektu_filtr_")]
        public double? PoradiVRamciVProjektuFiltr { get; set; }
        [Column("pocet_mist_realizace")]
        public double? PocetMistRealizace { get; set; }
    }
}
