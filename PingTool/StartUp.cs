using System.IO;
using System.Text;
using Serilog;
using Serilog.Events;

namespace PingTool
{
    static internal class StartUp
    {
        private static string _configFilePath;

        private static void CheckForConfigFile()
        {
            _configFilePath = "app.config";

            if (!File.Exists(_configFilePath))
            {
                StringBuilder sb = new StringBuilder();
                sb.AppendLine("<?xml version=\"1.0\" encoding=\"utf-8\" ?>");
                sb.AppendLine("<configuration>");
                sb.AppendLine("<appSettings>");

                sb.AppendLine("<add key=\"FileName\" value=\"PingLogFiles\\Logfile\"/>");
                sb.AppendLine("<add key=\"OutputTemplate\" value=\"[{Timestamp:dd-MM-yyyy HH:mm:ss}] {Message:lj}{NewLine}{Exception}\"/>");
                sb.AppendLine("<add key=\"Target\" value=\"8.8.8.8\"/>");
                sb.AppendLine("</appSettings>");
                sb.AppendLine("</configuration>");

                File.WriteAllText(_configFilePath, sb.ToString());
            }
        }

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