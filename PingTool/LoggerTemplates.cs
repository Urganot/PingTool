using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using Serilog;

namespace PingTool;

internal static class LoggerTemplates
{
    public static void OutputStartText(IEnumerable<IPAddress> targets, string saveFile, int interval, int pingTimeout)
    {
        Log.Information("Einstellungen");
        Log.Information("Datum: {ShortDateString}", DateTime.UtcNow.ToLocalTime().ToShortDateString());
        Log.Information("Uhrzeit: {ShortTimeString}", DateTime.UtcNow.ToLocalTime().ToShortTimeString());
        Log.Information("Ziel: {Join}", string.Join(", ", targets));
        Log.Information("Zeit zwischen Pings: {Interval}s", interval);
        Log.Information("Zeit bis  Ping Timeout: {PingTimeout}ms", pingTimeout);
        Log.Debug("Logfile wird gespeichert unter: {SaveFile}", saveFile);
        Log.Debug("Um einen zwischenstand anzuzeigen, drücke eine beliebige taste..");
        Log.Debug("Um das Logging zu beenden, drücke ESC");
    }

    public static void OutputIntermediateStatistics(Pings pings)
    {
        Log.Information("-----------------------------------------------------------------------");
        Log.Information("Zwischenstand für {$Target}:", pings.Target);
        OutputStatistics(pings);
        Log.Information("-----------------------------------------------------------------------");
    }

    public static void OutputEndStatistics(Pings pings)
    {
        Log.Information("-----------------------------------------------------------------------");
        Log.Information("Endergebnis für {$Target}:", pings.Target);
        OutputStatistics(pings);
        Log.Information("-----------------------------------------------------------------------");
    }

    public static void OutputStatistics(Pings pings)
    {
        Log.Information("Anzahl an Pinganfragen: {Count}", pings.Count);
        Log.Information("Anzahl an gesendeten Pings: {Count} => {Percentage:P2}", pings.SentPings.Count(),
            pings.SentPingsPercentage);
        Log.Information("Anzahl an erfolgreichen Pings: {Count} => {SuccessfulPingsPercentage:P2}",
            pings.SuccessfulPings.Count(), pings.SuccessfulPingsPercentage);
        Log.Information("Anzahl an nicht erfolgreichen Pings: {Count} => {PingsFailedPingsPercent:P2}",
            pings.FailedPings.Count(), pings.FailedPingsPercent);
        Log.Information("Anzahl an nicht gesendeten Pings: {Count} => {PingsExceptionPingsPercentage:P2}",
            pings.ExceptionPings.Count(), pings.ExceptionPingsPercentage);
        Log.Information("Minimale Latenz: {PingsMinLatency:F2}ms", pings.MinLatency);
        Log.Information("Maximale Latenz: {PingsMaxLatency:F2}ms", pings.MaxLatency);
        Log.Information("Durchschnittliche Latenz: {PingsAvgLatency:F2}ms", pings.AvgLatency);
    }
}