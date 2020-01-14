using HlidacStatu.Lib.Data.Dotace;
using log4net.Config;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

namespace SZIFParser
{
    class Program
    {
        static void Main(string[] args)
        {
            XmlConfigurator.Configure();
            string path = @"d:\Hlidac\dotace\szif\hlidac.json";
            JsonSerializer serializer = new JsonSerializer();


            DotaceService ds = new DotaceService();
            using (StreamReader file = File.OpenText(path))
            using (JsonReader reader = new JsonTextReader(file))
            {
                //var serializer = new JsonSerializer();
                int counter = 0;
                while (reader.Read())
                {
                    if (reader.TokenType == JsonToken.StartObject)
                    {
                        try
                        {

                            SZIFobj dot = serializer.Deserialize<SZIFobj>(reader);

                            
                            Dotace dotace = ds.Get(dot.IdDotace);

                            var listRozhodnuti = new List<HlidacStatu.Lib.Data.Dotace.Rozhodnuti>();

                            foreach (var roz in dot.Rozhodnuti)
                            {
                                listRozhodnuti.Add(new HlidacStatu.Lib.Data.Dotace.Rozhodnuti()
                                {
                                    CastkaPozadovana = roz.CastkaPozadovana,
                                    CastkaRozhodnuta = roz.CastkaRozhodnuta,
                                    Datum = roz.Datum,
                                    IcoPoskytovatele = roz.PoskytovatelIco,
                                    JePujcka = roz.JePujcka,
                                    NazevPoskytovatele = roz.PoskytovatelNazev,
                                    ZdrojFinanci = roz.ZdrojFinanci

                                });
                            }

                            if (dotace is null)
                            {
                                dotace = new Dotace()
                                {
                                    IdDotace = dot.IdDotace,
                                    DatumPodpisu = dot.PodpisDatum,

                                    IdProjektu = dot.IdProjektu,
                                    NazevProjektu = dot.NazevProjektu,
                                    KodProjektu = dot.KodProjektu,

                                    DotacniProgram = new HlidacStatu.Lib.Data.Dotace.DotacniProgram()
                                    {
                                        Id = dot.Program.Id,
                                        Kod = dot.Program.Kod,
                                        Nazev = dot.Program.Nazev,
                                        Url = dot.Program.Url
                                    },
                                    Prijemce = new HlidacStatu.Lib.Data.Dotace.Prijemce()
                                    {
                                        Ico = dot.Prijemce.Ico,
                                        JmenoPrijemce = dot.Prijemce.JmenoPrijemce,
                                        ObchodniJmeno = dot.Prijemce.PrijemceObchodniJmeno,
                                        PrijemceObecNazev = dot.Prijemce.PrijemceObecNazev,
                                        PrijemceOkresNazev = dot.Prijemce.PrijemceOkresNazev,
                                        PSC = dot.Prijemce.PrijemcePSC
                                    },
                                    Rozhodnuti = listRozhodnuti,
                                    NazevZdroje = dot.ZdrojNazev,
                                    UrlZdroje = dot.ZdrojUrl
                                };

                            }
                            else
                            {
                                var original = dotace;
                                var current = dot;
                                continue; //duplicitu přeskočit
                                // kvůli idObdobi je zde duplicita
                                //dotace.Rozhodnuti.Add(rozhodnuti);
                            }


                            

                                                        

                            dotace.Save();


                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine(ex);
                            //throw;
                        }
                    }
                }
            }

           
        }

        private static string RemoveWhiteSpaces(string text)
        {
            return string.Join("", text.Split(default(string[]), StringSplitOptions.RemoveEmptyEntries));
        }

        private static float ParseValue(string priceText)
        {
            priceText = RemoveWhiteSpaces(priceText);
            var match = Regex.Match(priceText, @"[0-9,]+");
            priceText = match.Value.Replace(",", ".");
            float.TryParse(priceText,NumberStyles.Float, CultureInfo.InvariantCulture, out var result);
            return result;
        }

        private static string GetTabValue(JObject jobj, string tab, string name)
        {

            try
            {
                return jobj["data"][tab]["data"]
                        .Where(jt => (string)jt["name"] == name)
                        .Select(jt => (string)jt["value"])
                        .FirstOrDefault();
            }
            catch 
            {
                return null;
            }
        }

        public static string NormalizeIco(string ico)
        {
            if (ico == null)
                return string.Empty;
            else if (ico.Contains("cz-"))
                return MerkIcoToICO(ico);
            else
                return ico.PadLeft(8, '0');
        }


        public static string MerkIcoToICO(string merkIco)
        {
            if (merkIco.ToLower().Contains("cz-"))
                merkIco = merkIco.Replace("cz-", "");

            return merkIco.PadLeft(8, '0');
        }

    }
}
