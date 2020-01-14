using CsvHelper;
using HlidacStatu.Lib.Data.Dotace;
using log4net;
using log4net.Config;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CedrParser
{
    class Program
    {
        //private static readonly ILog Logger = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        //private static Devmasters.Core.Logging.Logger logger = new Devmasters.Core.Logging.Logger("HlidacStatu.Downloader");
        static void Main(string[] args)
        {
            XmlConfigurator.Configure();
            string path = @"d:\Hlidac\dotace\output.csv";
            //string pathw = @"d:\Hlidac\dotace\ids.csv";

            DotaceService ds = new DotaceService();
            //using (StreamWriter streamWriter = new StreamWriter(pathw))

            int lineNbr = 0;
            using (StreamReader stream = new StreamReader(path))
            using (var csv = new CsvReader(stream))
            {
                csv.Configuration.Delimiter = ",";
                csv.Configuration.Encoding = Encoding.UTF8;
                csv.Read();
                csv.ReadHeader();
                //List<Dotace> dotaceList = new List<Dotace>();
                while (csv.Read())
                {
                    //streamWriter.WriteLine(csv.GetField("idDotace"));
                    Console.WriteLine(lineNbr++);
                    try
                    {
                        Dotace dotace = ds.Get(csv.GetField("idDotace"));

                        Rozhodnuti rozhodnuti = new Rozhodnuti();
                        if (!csv.GetField<bool>("rozhodnutiRefundaceIndikator")) //refundace není dotace
                        {
                            rozhodnuti.Id = csv.GetField("idRozhodnuti");
                            rozhodnuti.Datum = DateTime.Parse($"{csv.GetField<int?>("rozhodnutiRokRozhodnuti")}-1-1");
                            rozhodnuti.CastkaPozadovana = csv.GetField<float>("rozhodnutiCastkaPozadovana");
                            rozhodnuti.CastkaRozhodnuta = csv.GetField<float>("rozhodnutiCastkaRozhodnuta");
                            rozhodnuti.JePujcka = csv.GetField<bool>("rozhodnutiNavratnostIndikator");
                            rozhodnuti.NazevPoskytovatele = csv.GetField("rozhodnutiDotacePoskytovatelNazev");
                        }

                        DotacniProgram program = new DotacniProgram()
                        {
                            Id = csv.GetField("idOperacniProgram"),
                            Nazev = csv.GetField("dotaceOperacniProgramNazev"),
                            Kod = csv.GetField("dotaceOperacniProgramKod"),
                            Url = string.IsNullOrWhiteSpace(csv.GetField("dotaceIriOperacniProgram")) ? 
                                csv.GetField("dotaceIriProgram") : 
                                csv.GetField("dotaceIriOperacniProgram")
                        };

                        Prijemce prijemce = new Prijemce()
                        {
                            ObchodniJmeno = csv.GetField("prijemceObchodniJmeno"),
                            JmenoPrijemce = csv.GetField("prijemceJmenoPrijemce"),
                            Ico = NormalizeIco(csv.GetField("prijemceIco")),
                            PrijemceObecNazev = csv.GetField("prijemceObecNazev"),
                            PrijemceOkresNazev = csv.GetField("prijemceOkresNazev"),
                            RokNarozeni = csv.GetField<int?>("prijemceRokNarozeni")
                        };

                        if (dotace is null)
                        {
                            dotace = new Dotace()
                            {
                                IdDotace = $"CEDR-{csv.GetField("idDotace")}",
                                DatumPodpisu = csv.GetField<DateTime?>("dotacePodpisDatum"),
                                Rozhodnuti = new List<Rozhodnuti>() { rozhodnuti },
                                DotacniProgram = program,
                                Prijemce = prijemce,
                                DatumAktualizace = csv.GetField<DateTime?>("dotaceDTAktualizace"),
                                KodProjektu = csv.GetField("dotaceProjektKod"),
                                NazevProjektu = csv.GetField("dotaceProjektNazev"),
                                IdProjektu = csv.GetField("dotaceProjektIdentifikator"),
                                NazevZdroje = "CEDR",
                                UrlZdroje = "http://cedropendata.mfcr.cz/c3lod/cedr/resource/Dotace/" + csv.GetField("idDotace"),
                            };

                        }
                        else
                        {
                            // kvůli idObdobi je zde duplicita
                            if (dotace.Rozhodnuti.Any(r => r.Id == rozhodnuti.Id))
                                continue;
                            dotace.Rozhodnuti.Add(rozhodnuti);
                        }
                            
                        dotace.Save();
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine(csv.GetField("idObdobi"));
                        Console.WriteLine("error");
                        
                    }
                    

                }
               
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
