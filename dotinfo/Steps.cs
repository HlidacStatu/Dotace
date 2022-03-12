namespace DotInfo;

public static class Steps
{
    public static DateTime? ConvertRodneCisloToDate(string? rodneCislo)
    {
        if(string.IsNullOrWhiteSpace(rodneCislo))
            return null;

        rodneCislo = rodneCislo.Replace(" ", "").Replace("\\","").Replace("/","");

        if (rodneCislo.Length < 9 || rodneCislo.Length > 10)
            return null;

        if (!int.TryParse(rodneCislo.Substring(0, 2), out var year))
            return null;
        
        if (!int.TryParse(rodneCislo.Substring(2, 2), out var month))
            return null;
        
        if (!int.TryParse(rodneCislo.Substring(4, 2), out var day))
            return null;
        
        if (!int.TryParse(rodneCislo, out var celeCislo))
            return null;

        if (rodneCislo.Length == 9 && year > 53)
            return null;

        if (rodneCislo.Length == 10 && celeCislo % 11 != 0)
            return null;
        
        month = month > 12 ? month - 50 : month;
        
        year = year > 45 ? year + 1900 : year + 2000;

        try
        {
            return new DateTime(year, month, day);
        }
        catch
        {
            return null;
        }

    }
    
}