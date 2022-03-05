var files = Directory.GetFiles(".", "*.csv");
foreach (var file in files)
{
    Console.WriteLine(file);
    
}
Console.WriteLine("do you wish to proceed?");
Console.ReadLine();
foreach(var file in files)
{
    using var sw = new StreamWriter(file.Replace(".csv", "_fixed.csv"));
    using var sr = File.OpenText(file);
    
    string? line;

    while ((line = sr.ReadLine()) != null)
    {
        sw.WriteLine(line.Replace("\"\"",""));
    }
}