using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace DotInfo.Model
{
    [Keyless]
    [Table("dotace", Schema = "dotinfo")]
    public class DotInfo
    {
        [Column("url")]
        public string? Url { get; set; }
        [Column("dotace_datum_zverejneni")]
        public string? DotaceDatumZverejneni { get; set; }
        [Column("dotace_identifikator_dot_kod_is")]
        public string? DotaceIdentifikatorDotKodIs { get; set; }
        [Column("dotace_evidencni_cislo_dotace")]
        public string? DotaceEvidencniCisloDotace { get; set; }
        [Column("dotace_nazev_dotace")]
        public string? DotaceNazevDotace { get; set; }
        [Column("dotace_castka_pozadovana")]
        public string? DotaceCastkaPozadovana { get; set; }
        [Column("dotace_castka_schvalena")]
        public string? DotaceCastkaSchvalena { get; set; }
        [Column("dotace_ucel_dotace")]
        public string? DotaceUcelDotace { get; set; }
        [Column("dotace_kod_agendy")]
        public string? DotaceKodAgendy { get; set; }
        [Column("dotace_vyuziti_dotace")]
        public string? DotaceVyuzitiDotace { get; set; }
        [Column("dotace_rok_zahajeni")]
        public string? DotaceRokZahajeni { get; set; }
        [Column("dotace_rok_ukonceni")]
        public string? DotaceRokUkonceni { get; set; }
        [Column("dotace_lhuta_dosazeni_ucelu")]
        public string? DotaceLhutaDosazeniUcelu { get; set; }
        [Column("dotace_dosaz_stanoveneho_ucelu")]
        public string? DotaceDosazStanovenehoUcelu { get; set; }
        [Column("dotace_lhuta_pro_navraceni")]
        public string? DotaceLhutaProNavraceni { get; set; }
        [Column("dotace_mena")]
        public string? DotaceMena { get; set; }
        [Column("dotace_datum_podani_zadosti")]
        public string? DotaceDatumPodaniZadosti { get; set; }
        [Column("dotace_datum_vydani_rozhodnuti")]
        public string? DotaceDatumVydaniRozhodnuti { get; set; }
        [Column("dotace_poznamka")]
        public string? DotacePoznamka { get; set; }
        [Column("dotace_povinnosti_a_podminky")]
        public string? DotacePovinnostiAPodminky { get; set; }
        [Column("dotace_dotace_nfv")]
        public string? DotaceDotaceNfv { get; set; }
        [Column("dotace_forma_financovani_dotace")]
        public string? DotaceFormaFinancovaniDotace { get; set; }
        [Column("dotace_kod_obce_z_ruian")]
        public string? DotaceKodObceZRuian { get; set; }
        [Column("dotace_nazev_obce")]
        public string? DotaceNazevObce { get; set; }
        [Column("dotace_kod_casti_obce")]
        public string? DotaceKodCastiObce { get; set; }
        [Column("dotace_nazev_casti_obce")]
        public string? DotaceNazevCastiObce { get; set; }
        [Column("dotace_kod_lau_2")]
        public string? DotaceKodLau2 { get; set; }
        [Column("dotace_liniova_stavba")]
        public string? DotaceLiniovaStavba { get; set; }
        [Column("ucastnik_obchodni_jmeno")]
        public string? UcastnikObchodniJmeno { get; set; }
        [Column("ucastnik_ic_ucastnika_ic_zahranicni")]
        public string? UcastnikIcUcastnikaIcZahranicni { get; set; }
        [Column("ucastnik_prijemce_dotace_jmeno")]
        public string? UcastnikPrijemceDotaceJmeno { get; set; }
        [Column("ucastnik_rc_ucastnika")]
        public string? UcastnikRcUcastnika { get; set; }
        [Column("ucastnik_ulice_c_p_c_e_c_o_")]
        public string? UcastnikUliceCPCECO { get; set; }
        [Column("ucastnik_nazev_obce_doruc_posta")]
        public string? UcastnikNazevObceDorucPosta { get; set; }
        [Column("ucastnik_psc")]
        public string? UcastnikPsc { get; set; }
        [Column("ucastnik_pravnicka_osoba")]
        public string? UcastnikPravnickaOsoba { get; set; }
        [Column("ucastnik_kod_statu")]
        public string? UcastnikKodStatu { get; set; }
        [Column("ucastnik_dic")]
        public string? UcastnikDic { get; set; }
        [Column("ucastnik_datum_narozeni")]
        public string? UcastnikDatumNarozeni { get; set; }
        [Column("ucastnik_datum_vzniku")]
        public string? UcastnikDatumVzniku { get; set; }
        [Column("ucastnik_datum_zaniku")]
        public string? UcastnikDatumZaniku { get; set; }
        [Column("ucastnik_datum_zapisu")]
        public string? UcastnikDatumZapisu { get; set; }
        [Column("ucastnik_datum_vymazu")]
        public string? UcastnikDatumVymazu { get; set; }
        [Column("ucastnik_kod_obce")]
        public string? UcastnikKodObce { get; set; }
        [Column("ucastnik_kod_lau_2")]
        public string? UcastnikKodLau2 { get; set; }
        [Column("ucastnik_kod_pravni_formy")]
        public string? UcastnikKodPravniFormy { get; set; }
        [Column("ucastnik_kod_kraje")]
        public string? UcastnikKodKraje { get; set; }
        [Column("ucastnik_nazev_kraje")]
        public string? UcastnikNazevKraje { get; set; }
        [Column("ucastnik_kod_vusc")]
        public string? UcastnikKodVusc { get; set; }
        [Column("ucastnik_nazev_vusc")]
        public string? UcastnikNazevVusc { get; set; }
        [Column("ucastnik_kod_okresu")]
        public string? UcastnikKodOkresu { get; set; }
        [Column("ucastnik_nazev_okresu")]
        public string? UcastnikNazevOkresu { get; set; }
        [Column("ucastnik_kod_orp")]
        public string? UcastnikKodOrp { get; set; }
        [Column("ucastnik_nazev_orp")]
        public string? UcastnikNazevOrp { get; set; }
        [Column("ucastnik_kod_pou")]
        public string? UcastnikKodPou { get; set; }
        [Column("ucastnik_nazev_pou")]
        public string? UcastnikNazevPou { get; set; }
        [Column("ucastnik_kod_sop")]
        public string? UcastnikKodSop { get; set; }
        [Column("ucastnik_nazev_sop")]
        public string? UcastnikNazevSop { get; set; }
        [Column("ucastnik_kod_mop")]
        public string? UcastnikKodMop { get; set; }
        [Column("ucastnik_nazev_mop")]
        public string? UcastnikNazevMop { get; set; }
        [Column("ucastnik_kod_momc")]
        public string? UcastnikKodMomc { get; set; }
        [Column("ucastnik_nazev_momc")]
        public string? UcastnikNazevMomc { get; set; }
        [Column("ucastnik_kod_casti_obce")]
        public string? UcastnikKodCastiObce { get; set; }
        [Column("ucastnik_nazev_casti_obce")]
        public string? UcastnikNazevCastiObce { get; set; }
        [Column("ucastnik_kod_reg_soudrz_")]
        public string? UcastnikKodRegSoudrz { get; set; }
        [Column("ucastnik_nazev_reg_soudrz_")]
        public string? UcastnikNazevRegSoudrz { get; set; }
        [Column("ucastnik_kod_nadr_okresu")]
        public string? UcastnikKodNadrOkresu { get; set; }
        [Column("ucastnik_kod_statusu_obce")]
        public string? UcastnikKodStatusuObce { get; set; }
        [Column("identifikace_por_c_identifikator_osoby")]
        public string? IdentifikacePorCIdentifikatorOsoby { get; set; }
        [Column("identifikace_obchodni_jmeno")]
        public string? IdentifikaceObchodniJmeno { get; set; }
        [Column("identifikace_ic_osoby_ic_zahranicni")]
        public string? IdentifikaceIcOsobyIcZahranicni { get; set; }
        [Column("identifikace_jmeno")]
        public string? IdentifikaceJmeno { get; set; }
        [Column("identifikace_rc_ucastnika")]
        public string? IdentifikaceRcUcastnika { get; set; }
        [Column("identifikace_ulice_c_p_c_e_c_o_")]
        public string? IdentifikaceUliceCPCECO { get; set; }
        [Column("identifikace_nazev_obce_doruc_posta")]
        public string? IdentifikaceNazevObceDorucPosta { get; set; }
        [Column("identifikace_psc")]
        public string? IdentifikacePsc { get; set; }
        [Column("identifikace_kod_statu")]
        public string? IdentifikaceKodStatu { get; set; }
        [Column("identifikace_statutarni_organ")]
        public string? IdentifikaceStatutarniOrgan { get; set; }
        [Column("identifikace_dic")]
        public string? IdentifikaceDic { get; set; }
        [Column("identifikace_datum_narozeni")]
        public string? IdentifikaceDatumNarozeni { get; set; }
        [Column("identifikace_datum_vzniku")]
        public string? IdentifikaceDatumVzniku { get; set; }
        [Column("identifikace_datum_zaniku")]
        public string? IdentifikaceDatumZaniku { get; set; }
        [Column("identifikace_datum_zapisu")]
        public string? IdentifikaceDatumZapisu { get; set; }
        [Column("identifikace_datum_vymazu")]
        public string? IdentifikaceDatumVymazu { get; set; }
        [Column("identifikace_kod_obce")]
        public string? IdentifikaceKodObce { get; set; }
        [Column("identifikace_kod_lau_2")]
        public string? IdentifikaceKodLau2 { get; set; }
        [Column("identifikace_kod_pravni_formy")]
        public string? IdentifikaceKodPravniFormy { get; set; }
        [Column("identifikace_podil")]
        public string? IdentifikacePodil { get; set; }
        [Column("identifikace_kod_kraje")]
        public string? IdentifikaceKodKraje { get; set; }
        [Column("identifikace_nazev_kraje")]
        public string? IdentifikaceNazevKraje { get; set; }
        [Column("identifikace_kod_vusc")]
        public string? IdentifikaceKodVusc { get; set; }
        [Column("identifikace_nazev_vusc")]
        public string? IdentifikaceNazevVusc { get; set; }
        [Column("identifikace_kod_okresu")]
        public string? IdentifikaceKodOkresu { get; set; }
        [Column("identifikace_nazev_okresu")]
        public string? IdentifikaceNazevOkresu { get; set; }
        [Column("identifikace_kod_orp")]
        public string? IdentifikaceKodOrp { get; set; }
        [Column("identifikace_nazev_orp")]
        public string? IdentifikaceNazevOrp { get; set; }
        [Column("identifikace_kod_pou")]
        public string? IdentifikaceKodPou { get; set; }
        [Column("identifikace_nazev_pou")]
        public string? IdentifikaceNazevPou { get; set; }
        [Column("identifikace_kod_sop")]
        public string? IdentifikaceKodSop { get; set; }
        [Column("identifikace_nazev_sop")]
        public string? IdentifikaceNazevSop { get; set; }
        [Column("identifikace_kod_mop")]
        public string? IdentifikaceKodMop { get; set; }
        [Column("identifikace_nazev_mop")]
        public string? IdentifikaceNazevMop { get; set; }
        [Column("identifikace_kod_momc")]
        public string? IdentifikaceKodMomc { get; set; }
        [Column("identifikace_nazev_momc")]
        public string? IdentifikaceNazevMomc { get; set; }
        [Column("identifikace_kod_casti_obce")]
        public string? IdentifikaceKodCastiObce { get; set; }
        [Column("identifikace_nazev_casti_obce")]
        public string? IdentifikaceNazevCastiObce { get; set; }
        [Column("identifikace_kod_reg_soudrz_")]
        public string? IdentifikaceKodRegSoudrz { get; set; }
        [Column("identifikace_nazev_reg_soudrz_")]
        public string? IdentifikaceNazevRegSoudrz { get; set; }
        [Column("identifikace_kod_nadr_okresu")]
        public string? IdentifikaceKodNadrOkresu { get; set; }
        [Column("identifikace_kod_statusu_obce")]
        public string? IdentifikaceKodStatusuObce { get; set; }
        [Column("poskytovatel_poskytovatel_nazev_os")]
        public string? PoskytovatelPoskytovatelNazevOs { get; set; }
        [Column("poskytovatel_ic_poskytovatele")]
        public string? PoskytovatelIcPoskytovatele { get; set; }
        [Column("poskytovatel_ulice_c_p_c_e_c_o_")]
        public string? PoskytovatelUliceCPCECO { get; set; }
        [Column("poskytovatel_nazev_obce")]
        public string? PoskytovatelNazevObce { get; set; }
        [Column("poskytovatel_psc")]
        public string? PoskytovatelPsc { get; set; }
        [Column("poskytovatel_kod_financniho_mista")]
        public string? PoskytovatelKodFinancnihoMista { get; set; }
        [Column("poskytovatel_kod_obce")]
        public string? PoskytovatelKodObce { get; set; }
        [Column("poskytovatel_kod_casti_obce")]
        public string? PoskytovatelKodCastiObce { get; set; }
        [Column("poskytovatel_nazev_casti_obce")]
        public string? PoskytovatelNazevCastiObce { get; set; }
        [Column("kod_projektu")]
        public string? KodProjektu { get; set; }
    }
}
