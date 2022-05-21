using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Serilog;

namespace PingTool
{
    class LoggerTemplates
    {

        public static void OutputStartText(IPAddress target, string saveFile, int interval, int pingTimeout)
        {
            Log.Information("Einstellungen");
            Log.Information($"Datum: {DateTime.Now.ToShortDateString()}");
            Log.Information($"Uhrzeit: {DateTime.Now.ToShortTimeString()}");
            Log.Information($"Ziel: {target}");
            Log.Information($"Zeit zwischen Pings: {interval}s");
            Log.Information($"Zeit bis  Ping Timeout: {pingTimeout}ms");
            Log.Debug($"Logfile wird gespeichert unter: {saveFile}");
            Log.Debug($"Um einen zwischenstand anzuzeigen, drücke eine beliebige taste..");
            Log.Debug($"Um das Logging zu beenden, drücke ESC.");

        }

        public static void OutputIntermediateStatistics(Pings pings)
        {
            Log.Information("-----------------------------------------------------------------------");
            Log.Information("Zwischenstand:");
            OutputStatistics(pings);
            Log.Information("-----------------------------------------------------------------------");
        }
        public static void OutputEndStatistics(Pings pings)
        {
            Log.Information("-----------------------------------------------------------------------");
            Log.Information("Endergebnis:");
            OutputStatistics(pings);
            Log.Information("-----------------------------------------------------------------------");
        }

        public static void OutputStatistics(Pings pings)
        {

            Log.Information($"Anzahl an Pinganfragen: {pings.Count}");
            Log.Information($"Anzahl an gesendeten Pings: {pings.SentPings.Count} => {(pings.Count / Math.Max(1, pings.SentPings.Count)) * 100:F2}%");
            Log.Information($"Anzahl an erfolgreichen Pings: {pings.SuccessfulPings.Count} => {pings.SuccessfulPingsPercentage:F2}%");
            Log.Information($"Anzahl an nicht erfolgreichen Pings: {pings.FailedPings.Count} => {pings.FailedPingsPercent:F2}%");
            Log.Information($"Anzahl an nicht gesendeten Pings: {pings.ExceptionPings.Count} => {pings.ExceptionPingsPercentage:F2}");
            Log.Information($"Minimale Latenz: {pings.MinLatency:F2}ms");
            Log.Information($"MAximale Latenz: {pings.MaxLatency:F2}ms");
            Log.Information($"Durchschnittliche Latenz: {pings.AvgLatency:F2}ms");

        }
    }
}
