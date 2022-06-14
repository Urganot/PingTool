using System;
using System.Collections.Generic;
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
using System.Globalization;
using System.Reflection;
using System.Text;
using CommandLine;
using CsvHelper;
using Microsoft.Extensions.Options;

namespace PingTool
{
    class PingTool
    {
        private static string _defaultLogFileName = "PingLogFiles\\Logfile";

        private static readonly string _defaultOutputTemplate = "[{Timestamp:dd-MM-yyyy HH:mm:ss}] {Message:lj}{NewLine}{Exception}";

        public static int PingTimeout => ((int)Math.Floor(Interval * 1000 * 0.8));

        public static IPAddress Target;
        public static int Interval;
        public static bool OptionsValid = true;
        public static bool OutPutCsv;
        public static Pings Pings { get; set; } = new Pings();

        static void Main(string[] args)
        {
            Parser.Default.ParseArguments<Options>(args)
                .WithParsed(CommandLineParser.RunOptions)
                .WithNotParsed(CommandLineParser.HandleParseError);

            var logfileName = ConfigurationManager.AppSettings["FileName"] ?? _defaultLogFileName;
            var outputTemplate = ConfigurationManager.AppSettings["OutputTemplate"] ?? _defaultOutputTemplate;

            if (OptionsValid)
            {
                var saveFile = Path.Combine(Directory.GetCurrentDirectory(), logfileName);
                saveFile = Path.ChangeExtension(saveFile, "txt");

                StartUp.SetupLogger(outputTemplate, saveFile);

                LoggerTemplates.OutputStartText(Target, saveFile, Interval, PingTimeout);

                Log.Information("Pingvorgang gestartet.");
                ConsoleKey key = ConsoleKey.Spacebar;

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

                if (OutPutCsv) WriteCsvFile(Path.ChangeExtension(logfileName, "csv"), Pings);

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
}
