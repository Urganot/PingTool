using System.IO;
using System.Text;
using Serilog;
using Serilog.Events;

namespace PingTool
{
    internal static class StartUp
    {
        public static void SetupLogger(string outputTemplate, string saveFile)
        {
            Log.Logger = new LoggerConfiguration()
                .MinimumLevel.Debug()
                .WriteTo.Console(outputTemplate: outputTemplate)
                .WriteTo.File(path: saveFile, restrictedToMinimumLevel: LogEventLevel.Information,
                    outputTemplate: outputTemplate, rollingInterval: RollingInterval.Day, rollOnFileSizeLimit: true)
                .CreateLogger();
        }
    }
}