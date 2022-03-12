using System.Text.RegularExpressions;
using Common.IntermediateDb;

namespace Eufondy;

public static class Steps
{
    public static List<Rozhodnuti> CreateRozhodnuti(double? rozhodnutoCr,
        double? cerpanoCr,
        double? rozhodnutoEu,
        double? cerpanoEu)
    {
        cerpanoCr ??= 0;
        rozhodnutoCr ??= 0;
        cerpanoEu ??= 0;
        rozhodnutoEu ??= 0;

        List<Cerpani> czCerpani = new List<Cerpani>();
        if (cerpanoCr != 0)
        {
            czCerpani.Add(new Cerpani()
            {
                CastkaSpotrebovana = Convert.ToDecimal(Math.Round(cerpanoCr.Value, 2))
            });
        }

        List<Cerpani> euCerpani = new List<Cerpani>();
        if (cerpanoEu != 0)
        {
            euCerpani.Add(new Cerpani()
            {
                CastkaSpotrebovana = Convert.ToDecimal(Math.Round(cerpanoEu.Value, 2))
            });
        }

        List<Rozhodnuti> listRozhodnuti = new List<Rozhodnuti>();
        if (rozhodnutoCr != 0)
        {
            listRozhodnuti.Add(new Rozhodnuti()
            {
                CastkaRozhodnuta = Convert.ToDecimal(Math.Round(rozhodnutoCr.Value, 2)),
                Cerpani = czCerpani,
                Poskytovatel = "CZ"
            });
        }

        if (rozhodnutoEu != 0)
        {
            listRozhodnuti.Add(new Rozhodnuti()
            {
                CastkaRozhodnuta = Convert.ToDecimal(Math.Round(rozhodnutoEu.Value, 2)),
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