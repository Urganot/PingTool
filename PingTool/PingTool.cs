using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading;
using CommandLine;
using CsvHelper;
using PingTool.Configuration;
using PingTool.Logging;
using PingTool.Pings;
using Serilog;

namespace PingTool;

internal static class PingTool
{
    private static Dictionary<IPAddress, PingResult> PingResults { get; } = [];

    private static void Main(string[] args)
    {
        var parserResult = Parser.Default.ParseArguments<Options>(args);

        if (!parserResult.Errors.Any())
        {
            var configuration = parserResult.Value;

            LoggerUtils.SetupLogger(configuration);

            LoggerTemplates.OutputStartText(
                configuration.Targets,
                configuration.Interval,
                configuration.PingTimeout
            );

            foreach (var target in configuration.Targets)
            {
                PingResults.Add(target, new PingResult { Target = target });
            }

            Log.Information("Pingvorgang gestartet");

            State state;

            do
            {
                state = GetState(configuration);

                foreach (var (target, pings) in PingResults)
                {
                    pings.Add(Ping.Send(target, configuration.PingTimeout));

                    if (state == State.PrintIntermediate)
                        pings.OutputIntermediateStatistics();
                }

                Thread.Sleep(configuration.Interval * 1000);
            } while (state != State.EndRun);

            foreach (var (_, pings) in PingResults)
            {
                pings.OutputEndStatistics();
            }


            if (configuration.OutputCsv)
                WriteCsvFile(Path.ChangeExtension(LoggerUtils.CurrentLogFilePath, "csv"), PingResults);

            Log.CloseAndFlush();
        }

        Console.WriteLine("Um das Fenster zu schließen, drücke irgendeine Taste.");
        Console.ReadKey();
    }

    private static State GetState(Options configuration)
    {
        State state;
        if (!Console.KeyAvailable)
            state = State.Default;
        else if (configuration.CancelKeys.Contains(Console.ReadKey(true).Key))
            state = State.EndRun;
        else
            state = State.PrintIntermediate;
        return state;
    }

    private static void WriteCsvFile(string fileName, Dictionary<IPAddress, PingResult> pingResults)
    {
        using (var writer = new StreamWriter(fileName))
        {
            using (var csv = new CsvWriter(writer, CultureInfo.InvariantCulture))
            {
                foreach (var (_, pings) in pingResults)
                {
                    csv.WriteRecords(pings.ListOfPings);
                }
            }
        }
    }

    private enum State
    {
        Default,
        PrintIntermediate,
        EndRun
    }
}