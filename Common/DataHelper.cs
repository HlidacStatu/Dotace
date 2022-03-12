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
}