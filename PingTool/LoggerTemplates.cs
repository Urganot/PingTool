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

        public static void OutputStatistics(PingResults pingResult)
        {
            Log.Information("Statistiken:");
            Log.Information($"Anzahl an Pinganfragen: {pingResult.AmountOfTriedPings}");
            Log.Information($"Anzahl an gesendeten Pings: {pingResult.AmountOfSendPings} => {pingResult.AmountOfSendPingsPercent:F2}%");
            Log.Information($"Anzahl an erfolgreichen Pings: {pingResult.AmountOfSuccess} => {pingResult.AmountOfSuccessfulPingsPercent:F2}%");
            Log.Information($"Anzahl an nicht erfolgreichen Pings: {pingResult.AmountOfOther} => {pingResult.AmountOfOtherPingsPercent:F2}%");
            Log.Information($"Anzahl an nicht gesendeten Pings: {pingResult.AmountOfExceptions} => {pingResult.AmountOfExceptionsPercent:F2}");
            Log.Information($"Minimale Latenz: {pingResult.MinLatency:F2}ms");
            Log.Information($"MAximale Latenz: {pingResult.MaxLatency:F2}ms");
            Log.Information($"Durchschnittliche Latenz: {pingResult.AverageLatency:F2}ms");

        }
    }
}
