using HlidacStatu.Lib.Data.Dotace;
using log4net;
using log4net.Config;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace DotInfoParser
{
    class Program
    {
        //private static readonly ILog Logger = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        //private static Devmasters.Core.Logging.Logger logger = new Devmasters.Core.Logging.Logger("HlidacStatu.Downloader");
        static void Main(string[] args)
        {
            XmlConfigurator.Configure();
            string path = @"d:\Hlidac\dotace\dotInfo.json";
            //string pathw = @"d:\Hlidac\dotace\names.csv";

            DotaceService ds = new DotaceService();
            //using (StreamWriter sw = File.AppendText(pathw))
            using (StreamReader file = File.OpenText(path))
            using (JsonTextReader reader = new JsonTextReader(file))
            {
                //var serializer = new JsonSerializer();
                int counter = 0;
                while (reader.Read())
                {
                    if (reader.TokenType == JsonToken.StartObject)
                    {
                        try
                        {
                            JObject o2 = (JObject)JToken.ReadFrom(reader);
                            // Deserialize each object from the stream individually and process it
                            //var data = serializer.Deserialize<playdata>(reader);
                            //ProcessPlayData(playdata);

                            if ((string)o2["type"] == "detail")
                            {
                                Console.WriteLine(counter++);
                            }
                            else
                            {
                                continue;
                            }

                            string dotaceId = $"DotInfo-{GetTabValue(o2, "tab1", "Identifikátor dot. / Kód IS")}"; // o2["data"]["tab1"]["data"].Where(jt => (string)jt["name"] == "Identifikátor dot. / Kód IS").Select(jt => (string)jt["value"]).FirstOrDefault();
                            dotaceId = Devmasters.Core.TextUtil.NormalizeToURL(dotaceId); //musíme to normalizovat

                            Dotace dotace = ds.Get(dotaceId);
                            Rozhodnuti rozhodnuti = new Rozhodnuti()
                            {
                                CastkaPozadovana = ParseValue(GetTabValue(o2, "tab1", "Částka požadovaná")),
                                CastkaRozhodnuta = ParseValue(GetTabValue(o2, "tab1", "Částka schválená")),
                                Datum = HlidacStatu.Lib.Validators.DateInText(
                                    RemoveWhiteSpaces(GetTabValue(o2, "tab1", "Datum vydání rozhodnutí")
                                    )),
                                NazevPoskytovatele = GetTabValue(o2, "tab4", "Poskytovatel - Název OS"),
                                IcoPoskytovatele = GetTabValue(o2, "tab4", "IČ poskytovatele")
                            };

                            DotacniProgram program = new DotacniProgram()
                            {
                                Nazev = GetTabValue(o2, "tab1", "Účel dotace")
                            };

                            Prijemce prijemce = new Prijemce()
                            {
                                ObchodniJmeno = GetTabValue(o2, "tab2", "Obchodní jméno"),
                                JmenoPrijemce = GetTabValue(o2, "tab2", "Příjemce dotace - Jméno"),
                                Ico = NormalizeIco(GetTabValue(o2, "tab2", "IČ účastníka / IČ zahraniční")),
                                PrijemceObecNazev = GetTabValue(o2, "tab2", "Název obce / Doruč. pošta"),
                                PrijemceOkresNazev = GetTabValue(o2, "tab2", "Název okresu"),
                                RokNarozeni = HlidacStatu.Lib.Validators.DateInText(
                                        RemoveWhiteSpaces(GetTabValue(o2, "tab2", "Datum narození")
                                        ))?.Year,
                                PSC = GetTabValue(o2, "tab2", "PSČ")
                            };

                            if (dotace is null)
                            {
                                dotace = new Dotace()
                                {
                                    IdDotace = dotaceId,
                                    DatumPodpisu = HlidacStatu.Lib.Validators.DateInText(
                                        RemoveWhiteSpaces(GetTabValue(o2, "tab1", "Datum vydání rozhodnutí"))),
                                    
                                    IdProjektu = GetTabValue(o2, "tab1", "Identifikátor dot. / Kód IS"),
                                    NazevProjektu = GetTabValue(o2, "tab1", "Název dotace"),
                                    
                                    DotacniProgram = program,
                                    Prijemce = prijemce,
                                    Rozhodnuti = new List<Rozhodnuti>() { rozhodnuti },
                                    NazevZdroje = "DotInfo",
                                    UrlZdroje = (string)o2["url"]
                                };
                                
                            }
                            else
                            {
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
