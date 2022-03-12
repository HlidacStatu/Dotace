using Common.IntermediateDb;
using Serilog;

namespace Common;

public static class DotaceRepo
{
    public static async Task SaveDotaceToDb(IEnumerable<Dotace> dotaceResults,
        ILogger appLogger,
        string cnnString,
        int chunkSize = 1000)
    {
        var dotaceChunks = dotaceResults.Chunk(chunkSize);

        int chunkNumber = 0;
        foreach (var recordChunk in dotaceChunks)
        {
            appLogger.Debug($"Uploading chunk nbr {chunkNumber++}");

            await using var db = new IntermediateDbContext(cnnString);

            db.Dotace.AddRange(recordChunk);
            await db.SaveChangesAsync();
        }
    }
}