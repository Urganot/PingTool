using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Net;
using System.Threading;
using CommandLine;
using CsvHelper;
using Serilog;

namespace PingTool;

internal static class PingTool
{
    public static IEnumerable<IPAddress> Targets;
    public static int Interval;
    public static bool OptionsValid = true;
    public static bool OutPutCsv;
    public static string LogFileName;
    public static string OutputTemplate;

    private static int PingTimeout => (int)Math.Floor(Interval * 1000 * 0.8);

    public static Dictionary<IPAddress, Pings> PingsByTarget { get; } = [];
    
    private static List<ConsoleKey> CancelKeys = [ConsoleKey.Escape, ConsoleKey.X];

    private static void Main(string[] args)
    {
        Parser.Default.ParseArguments<Options>(args)
            .WithParsed(CommandLineParser.RunOptions)
            .WithNotParsed(CommandLineParser.HandleParseError);

        if (OptionsValid)
        {
            var saveFile = Path.Combine(Directory.GetCurrentDirectory(), LogFileName);
            saveFile = Path.ChangeExtension(saveFile, "txt");

            StartUp.SetupLogger(OutputTemplate, saveFile);

            LoggerTemplates.OutputStartText(Targets, saveFile, Interval, PingTimeout);

            foreach (var target in Targets)
            {
                PingsByTarget.Add(target, new Pings {Target = target});
            }
            
            Log.Information("Pingvorgang gestartet");
            
            var key = ConsoleKey.Spacebar;

            do
            {
                foreach (var (target, pings) in PingsByTarget)
                {
                    pings.Add(Ping.Send(target, PingTimeout));
                    
                    if (Console.KeyAvailable)
                    {
                        key = Console.ReadKey(true).Key;
                        if (CancelKeys.Contains(key)) LoggerTemplates.OutputIntermediateStatistics(pings);
                        else LoggerTemplates.OutputEndStatistics(pings);
                    }
                }
                
                Thread.Sleep(Interval * 1000);

            } while (!CancelKeys.Contains(key));

            if (OutPutCsv) WriteCsvFile(Path.ChangeExtension(LogFileName, "csv"), PingsByTarget);

            Log.CloseAndFlush();
        }

        Console.WriteLine("Um das Fenster zu schließen, drücke irgendeine Taste.");
        Console.ReadKey();
    }

    private static void WriteCsvFile(string fileName, Dictionary<IPAddress, Pings> pingsByTarget)
    {
        foreach (var (_, pings) in pingsByTarget)
        {
            using (var writer = new StreamWriter(fileName))
            {
                using (var csv = new CsvWriter(writer, CultureInfo.InvariantCulture))
                {
                    csv.WriteRecords(pings.pings);
                }
            }
        }
    }
}