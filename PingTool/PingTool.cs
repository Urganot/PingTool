using System;
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
    public static IPAddress Target;
    public static int Interval;
    public static bool OptionsValid = true;
    public static bool OutPutCsv;
    public static string LogFileName;
    public static string OutputTemplate;

    private static int PingTimeout => (int)Math.Floor(Interval * 1000 * 0.8);

    public static Pings Pings { get; set; } = new();

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

            LoggerTemplates.OutputStartText(Target, saveFile, Interval, PingTimeout);

            Log.Information("Pingvorgang gestartet.");
            var key = ConsoleKey.Spacebar;

            do
            {
                Pings.Add(Ping.Send(Target, PingTimeout));

                Thread.Sleep(Interval * 1000);

                if (Console.KeyAvailable)
                {
                    key = Console.ReadKey(true).Key;
                    if (key != ConsoleKey.Escape) LoggerTemplates.OutputIntermediateStatistics(Pings);
                    else LoggerTemplates.OutputEndStatistics(Pings);
                }
            } while (key != ConsoleKey.Escape);

            if (OutPutCsv) WriteCsvFile(Path.ChangeExtension(LogFileName, "csv"), Pings);

            Log.CloseAndFlush();
        }

        Console.WriteLine("Um das Fenster zu schließen, drücke irgendeine Taste.");
        Console.ReadKey();
    }

    private static void WriteCsvFile(string fileName, Pings pings)
    {
        using (var writer = new StreamWriter(fileName))
        using (var csv = new CsvWriter(writer, CultureInfo.InvariantCulture))
        {
            csv.WriteRecords(pings.pings);
        }
    }
}