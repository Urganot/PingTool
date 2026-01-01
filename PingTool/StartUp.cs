using Serilog;
using Serilog.Events;

namespace PingTool;

internal static class StartUp
{
    public static void SetupLogger(string outputTemplate, string saveFile)
    {
        Log.Logger = new LoggerConfiguration()
            .MinimumLevel.Debug()
            .WriteTo.Console(outputTemplate: outputTemplate)
            .WriteTo.File(saveFile, LogEventLevel.Information,
                outputTemplate, rollingInterval: RollingInterval.Day, rollOnFileSizeLimit: true)
            .CreateLogger();
    }
}