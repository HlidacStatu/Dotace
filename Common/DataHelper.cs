namespace Common;

public static class DataHelper
{
    public static decimal? GetPriceFromText(string? text)
    {
        if (string.IsNullOrWhiteSpace(text))
            return null;
        
        var price = text.Replace(" ", "").Replace(".", ",");
        price = new string(price.Where(c => char.IsDigit(c) || c == ',').ToArray());

        if (decimal.TryParse(price, out var castkaCr))
        {
            return castkaCr;
        }

        return null;
    }
    
    public static decimal ConvertDoubleToMoney(double? value)
    {
        if (value == null)
            return 0;
        
        return Convert.ToDecimal(Math.Round(value.Value, 2));
    }

    public static string GetDbConnectionString()
    {
        string envVariable = "POSTGRES_CONNECTION_DOTNET";
        var cnnString = Environment.GetEnvironmentVariable(envVariable);
        if (cnnString == null)
        {
            throw new Exception($"Nenalezena {envVariable} environment proměnná");
        }

        return cnnString;
    }
}