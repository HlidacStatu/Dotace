using System.Text.RegularExpressions;
using Common.IntermediateDb;

namespace Eufondy;

public static class Steps
{
    public static List<Rozhodnuti> CreateRozhodnuti(decimal rozhodnutoCr,
        decimal cerpanoCr,
        decimal rozhodnutoEu,
        decimal cerpanoEu)
    {
        List<Cerpani> czCerpani = new List<Cerpani>();
        if (cerpanoCr != 0)
        {
            czCerpani.Add(new Cerpani()
            {
                CastkaSpotrebovana = Math.Round(cerpanoCr, 2)
            });
        }

        List<Cerpani> euCerpani = new List<Cerpani>();
        if (cerpanoEu != 0)
        {
            euCerpani.Add(new Cerpani()
            {
                CastkaSpotrebovana = Math.Round(cerpanoEu, 2)
            });
        }

        List<Rozhodnuti> listRozhodnuti = new List<Rozhodnuti>();
        if (rozhodnutoCr != 0)
        {
            listRozhodnuti.Add(new Rozhodnuti()
            {
                CastkaRozhodnuta = Math.Round(rozhodnutoCr, 2),
                Cerpani = czCerpani,
                Poskytovatel = "CZ"
            });
        }

        if (rozhodnutoEu != 0)
        {
            listRozhodnuti.Add(new Rozhodnuti()
            {
                CastkaRozhodnuta = Math.Round(rozhodnutoEu, 2),
                Cerpani = euCerpani,
                Poskytovatel = "EU"
            });
        }

        return listRozhodnuti;
    }

    public static string GetPscFromAddress(string address)
    {
        var psc = Regex.Match(address, @"^(\d{3}) ?(\d{2})");
        if (psc.Success)
        {
            return $"{psc.Groups[1].Value}{psc.Groups[2].Value}";
        }

        return "";
    }
}