using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading;
using CommandLine;
using CsvHelper;
using Serilog;

namespace PingTool;

internal static class PingTool
{
    public static Dictionary<IPAddress, Pings> PingsByTarget { get; } = [];

    private static void Main(string[] args)
    {
        var parserResult = Parser.Default.ParseArguments<Options>(args);

        if (!parserResult.Errors.Any())
        {
            var configuration = parserResult.Value;

            var saveFile = Path.Combine(Directory.GetCurrentDirectory(), configuration.LogFileName);
            saveFile = Path.ChangeExtension(saveFile, "txt");

            StartUp.SetupLogger(configuration.OutputTemplate, saveFile);

            LoggerTemplates.OutputStartText(configuration.Targets, saveFile, configuration.Interval,
                configuration.PingTimeout);

            foreach (var target in configuration.Targets)
            {
                PingsByTarget.Add(target, new Pings { Target = target });
            }

            Log.Information("Pingvorgang gestartet");

            State state;

            do
            {
                state = GetState(configuration);

                foreach (var (target, pings) in PingsByTarget)
                {
                    pings.Add(Ping.Send(target, configuration.PingTimeout));

                    if (state == State.PrintIntermediate)
                        pings.OutputIntermediateStatistics();
                }

                Thread.Sleep(configuration.Interval * 1000);
            } while (state != State.EndRun);

            foreach (var (_, pings) in PingsByTarget)
            {
                pings.OutputEndStatistics();
            }


            if (configuration.OutputCsv)
                WriteCsvFile(Path.ChangeExtension(configuration.LogFileName, "csv"), PingsByTarget);

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

    private static void WriteCsvFile(string fileName, Dictionary<IPAddress, Pings> pingsByTarget)
    {
        using (var writer = new StreamWriter(fileName))
        {
            using (var csv = new CsvWriter(writer, CultureInfo.InvariantCulture))
            {
                foreach (var (_, pings) in pingsByTarget)
                {
                    csv.WriteRecords(pings.ListOfPings);
                }
            }
        }
    }

    internal enum State
    {
        Default,
        PrintIntermediate,
        EndRun
    }
}