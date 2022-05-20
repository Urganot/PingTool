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
            Log.Debug($"Um das Logging zu beenden, drücke irgendeine Taste.");

        }

        public static void OutputStatistics(Pings pings)
        {
            Log.Information("Statistiken:");
            Log.Information($"Anzahl an Pinganfragen: {pings.Count}");
            Log.Information($"Anzahl an gesendeten Pings: {pings.SentPings.Count} => {pings.Count / pings.SentPings.Count * 100:F2}%");
            Log.Information($"Anzahl an erfolgreichen Pings: {pings.SuccessfulPings.Count} => {pings.SuccessfulPings.Count / pings.Count * 100:F2}%");
            Log.Information($"Anzahl an nicht erfolgreichen Pings: {pings.UnknownPings.Count} => {pings.UnknownPings.Count / pings.Count * 100:F2}%");
            Log.Information($"Anzahl an nicht gesendeten Pings: {pings.ExceptionPings.Count} => {pings.ExceptionPings.Count / pings.Count * 100:F2}");
            Log.Information($"Minimale Latenz: {pings.MinLatency:F2}ms");
            Log.Information($"MAximale Latenz: {pings.MaxLatency:F2}ms");
            Log.Information($"Durchschnittliche Latenz: {pings.AvgLatency:F2}ms");

        }
    }
}
