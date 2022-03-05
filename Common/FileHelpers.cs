namespace Common;

public static class FileHelpers
{
    public static async Task RemoveFiles(string folder)
    {
        var files = Directory.GetFiles(folder);
        foreach (var file in files)
        {
            File.Delete(file);
        }
    }
}