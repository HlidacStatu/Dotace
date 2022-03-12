using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Eufondy.Model
{
    [Keyless]
    [Table("dotace2020", Schema = "eufondy")]
    public partial class Dotace2020
    {
        [Column("kod_projektu")]
        public string? KodProjektu { get; set; }
        [Column("id")]
        public long? Id { get; set; }
        [Column("id_vyzva")]
        public long? IdVyzva { get; set; }
        [Column("naz")]
        public string? Naz { get; set; }
        [Column("popis")]
        public string? Popis { get; set; }
        [Column("problem")]
        public string? Problem { get; set; }
        [Column("cil")]
        public string? Cil { get; set; }
        [Column("datum_zahajeni")]
        public string? DatumZahajeni { get; set; }
        [Column("datum_ukonceni_predp")]
        public string? DatumUkonceniPredp { get; set; }
        [Column("datum_ukonceni_skut")]
        public string? DatumUkonceniSkut { get; set; }
        [Column("suk")]
        public string? Suk { get; set; }
        [Column("zadatel_nazev")]
        public string? ZadatelNazev { get; set; }
        [Column("zadatel_ico")]
        public double? ZadatelIco { get; set; }
        [Column("zadatel_pravni_forma")]
        public string? ZadatelPravniForma { get; set; }
        [Column("cile_projektu")]
        public long? CileProjektu { get; set; }
        [Column("financovani_czv")]
        public double? FinancovaniCzv { get; set; }
        [Column("financovani_eu")]
        public double? FinancovaniEu { get; set; }
        [Column("financovani_cnv")]
        public double? FinancovaniCnv { get; set; }
        [Column("financovani_sn")]
        public double? FinancovaniSn { get; set; }
        [Column("financovani_s")]
        public double? FinancovaniS { get; set; }
        [Column("financovani_esif")]
        public double? FinancovaniEsif { get; set; }
        [Column("financovani_cv")]
        public double? FinancovaniCv { get; set; }
        [Column("cilove_skupiny")]
        public string? CiloveSkupiny { get; set; }
        [Column("zadatel_obec")]
        public string? ZadatelObec { get; set; }
        [Column("zadatel_okres")]
        public string? ZadatelOkres { get; set; }
        [Column("zadatel_psc")]
        public string? ZadatelPsc { get; set; }
    }
}
