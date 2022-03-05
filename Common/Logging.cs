using Serilog;
using Serilog.Core;

namespace Common;

public class Logging
{
    public static ILogger CreateLogger(string filename)
    {
        return new LoggerConfiguration()
            .MinimumLevel.Debug()
            .WriteTo.Console()
            .WriteTo.File(filename)
            .CreateLogger();
    }
        
        
    
}