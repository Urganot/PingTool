using System;
using System.IO;
using System.Net;
using System.Net.NetworkInformation;
using System.Threading;
using Microsoft.Extensions.Configuration;
using Serilog;
using Serilog.Core;
using Serilog.Debugging;
using Serilog.Events;
using System.Configuration;
using System.Dynamic;
using System.Reflection;
using System.Text;

namespace PingTool
{
    class PingTool
    {
        private static string _defaultLogFileName = "PingLogFiles\\Logfile";

        private static string _defaultoutputTemplate = "[{Timestamp:dd-MM-yyyy HH:mm:ss}] {Message:lj}{NewLine}{Exception}";

        private static string _defaultTarget = "8.8.8.8";

        private static string _configFilePath;

        private static int _defaultTimeBetweenPings = 1;

        private static PingResults _pingResult;

        public static int PingTimeout => ((int) Math.Floor(_defaultTimeBetweenPings * 1000 * 0.8));

        static void Main(string[] args)
        {

            var logfileName = ConfigurationManager.AppSettings["FileName"] ?? _defaultLogFileName;
            var outputTemplate = ConfigurationManager.AppSettings["OutputTemplate"] ?? _defaultoutputTemplate;
            var target = IPAddress.Parse(ConfigurationManager.AppSettings["Target"] ?? _defaultTarget);
            _pingResult = new PingResults();


            var saveFile = Path.Combine(Directory.GetCurrentDirectory(), logfileName);
            saveFile = Path.ChangeExtension(saveFile, "txt");

            SetupLogger(outputTemplate, saveFile);

            OutputStartText(target, saveFile);

            Log.Information("Pingvorgang gestartet.");

            while (!Console.KeyAvailable)
            {
                PingHost(target);
                Thread.Sleep(_defaultTimeBetweenPings * 1000);
            }
            Console.ReadKey();

            OutputStatistics();

            Log.CloseAndFlush();

            Console.WriteLine("Um das Fenster zu schließen, drücke irgendeine Taste.");
            Console.ReadKey();
        }

        private static void SetupLogger(string outputTemplate, string saveFile)
        {
            Log.Logger = new LoggerConfiguration()
                .MinimumLevel.Debug()
                .WriteTo.Console(outputTemplate: outputTemplate)
                .WriteTo.File(path: saveFile, restrictedToMinimumLevel: LogEventLevel.Information,
                    outputTemplate: outputTemplate, rollingInterval: RollingInterval.Day)
                .CreateLogger();
        }

        private static void OutputStartText(IPAddress target, string saveFile)
        {
            Log.Information("Einstellungen");
            Log.Information($"Datum: {DateTime.Now.ToShortDateString()}");
            Log.Information($"Uhrzeit: {DateTime.Now.ToShortTimeString()}");
            Log.Information($"Ziel: {target}");
            Log.Information($"Zeit zwischen Pings: {_defaultTimeBetweenPings}s");
            Log.Information($"Zeit bis  Ping Timeout: {PingTimeout}ms");
            Log.Debug($"Logfile wird gespeichert unter: {saveFile}");
            Log.Debug($"Um das Logging zu beenden, drücke irgendeine Taste.");

        }

        private static void OutputStatistics()
        {
            Log.Information("Statistiken:");
            Log.Information($"Anzahl an Pinganfragen: {_pingResult.AmountOfTriedPings}");
            Log.Information($"Anzahl an gesendeten Pings: {_pingResult.AmountOfSendPings} => {_pingResult.AmountOfSendPingsPercent:F2}%");
            Log.Information($"Anzahl an erfolgreichen Pings: {_pingResult.AmountOfSuccess} => {_pingResult.AmountOfSuccessfulPingsPercent:F2}%");
            Log.Information($"Anzahl an nicht erfolgreichen Pings: {_pingResult.AmountOfOther} => {_pingResult.AmountOfOtherPingsPercent:F2}%");
            Log.Information($"Anzahl an nicht gesendeten Pings: {_pingResult.AmountOfExceptions} => {_pingResult.AmountOfExceptionsPercent:F2}");
            Log.Information($"Durchschnittliche Latenz: {_pingResult.AverageLatency:F2}ms");

        }

        private static void CheckForConfigFile()
        {
            _configFilePath = "app.config";

            if (!File.Exists(_configFilePath))
            {
                System.Text.StringBuilder sb = new StringBuilder();
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

        public static void PingHost(IPAddress ipAddress)
        {
            Ping pinger = null;

            try
            {
                pinger = new Ping();
                _pingResult.AmountOfTriedPings++;
                PingReply reply = pinger.Send(ipAddress, PingTimeout);
                _pingResult.AmountOfSendPings++;

                if (reply != null)
                {
                    Log.Information($"Pinged {reply.Address} Status: {reply.Status} Latency: {reply.RoundtripTime}ms");
                    _pingResult.SumOfLatency += reply.RoundtripTime;


                    if (reply.Status == IPStatus.Success)
                        _pingResult.AmountOfSuccess++;
                    else
                        _pingResult.AmountOfOther++;
                }
                else
                {
                    _pingResult.AmountOfOther++;
                }
            }
            catch (PingException pEx)
            {
                Log.Information($"Pinged {ipAddress} Status: Exception Message:{pEx.Message} => {pEx.InnerException?.Message ?? "No more detailed message"}");
                _pingResult.AmountOfExceptions++;
            }
            finally
            {
                pinger?.Dispose();
            }
        }

    }
}
