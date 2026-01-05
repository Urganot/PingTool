using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using PingTool.Pings;
using Serilog;

namespace PingTool.Logging;

internal static class LoggerTemplates
{
    public static void OutputStartText(IEnumerable<IPAddress> targets, int interval, int pingTimeout)
    {
        Log.Information("Einstellungen");
        Log.Information("Datum: {ShortDateString}", DateTime.UtcNow.ToLocalTime().ToShortDateString());
        Log.Information("Uhrzeit: {ShortTimeString}", DateTime.UtcNow.ToLocalTime().ToShortTimeString());
        Log.Information("Ziel: {Join}", string.Join(", ", targets));
        Log.Information("Zeit zwischen Pings: {Interval}s", interval);
        Log.Information("Zeit bis  Ping Timeout: {PingTimeout}ms", pingTimeout);

        if (Path.Exists(LoggerUtils.CurrentLogFilePath))
            Log.Debug("Logfile wird gespeichert unter: \"{LogFilePath}\"", LoggerUtils.CurrentLogFilePath);
        else
        {
            Log.Warning(
                "Pfad zu Logfile sollte \"{LogFilePath}\" sein, aber dieser existiert nicht",
                LoggerUtils.CurrentLogFilePath
            );
        }

        Log.Debug("Um einen zwischenstand anzuzeigen, drücke eine beliebige taste..");
        Log.Debug("Um das Logging zu beenden, drücke ESC");
    }

    public static void OutputIntermediateStatistics(PingResult pingResult)
    {
        Log.Information("-----------------------------------------------------------------------");
        Log.Information("Zwischenstand für {$Target}:", pingResult.Target);
        OutputStatistics(pingResult);
        Log.Information("-----------------------------------------------------------------------");
    }

    public static void OutputEndStatistics(PingResult pingResult)
    {
        Log.Information("-----------------------------------------------------------------------");
        Log.Information("Endergebnis für {$Target}:", pingResult.Target);
        OutputStatistics(pingResult);
        Log.Information("-----------------------------------------------------------------------");
    }

    public static void OutputStatistics(PingResult pingResult)
    {
        Log.Information("Anzahl an Pinganfragen: {Count}", pingResult.Count);
        Log.Information(
            "Anzahl an gesendeten Pings: {Count} => {Percentage:P2}",
            pingResult.SentPings.Count(),
            pingResult.SentPingsPercentage
        );
        Log.Information(
            "Anzahl an erfolgreichen Pings: {Count} => {SuccessfulPingsPercentage:P2}",
            pingResult.SuccessfulPings.Count(),
            pingResult.SuccessfulPingsPercentage
        );
        Log.Information(
            "Anzahl an nicht erfolgreichen Pings: {Count} => {PingsFailedPingsPercent:P2}",
            pingResult.FailedPings.Count(),
            pingResult.FailedPingsPercent
        );
        Log.Information(
            "Anzahl an nicht gesendeten Pings: {Count} => {PingsExceptionPingsPercentage:P2}",
            pingResult.ExceptionPings.Count(),
            pingResult.ExceptionPingsPercentage
        );
        Log.Information("Minimale Latenz: {PingsMinLatency:F2}ms", pingResult.MinLatency);
        Log.Information("Maximale Latenz: {PingsMaxLatency:F2}ms", pingResult.MaxLatency);
        Log.Information("Durchschnittliche Latenz: {PingsAvgLatency:F2}ms", pingResult.AvgLatency);
    }
}