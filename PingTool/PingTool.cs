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
using System.Reflection;
using System.Text;
using CommandLine;
using Microsoft.Extensions.Options;

namespace PingTool
{
    class PingTool
    {
        private static string _defaultLogFileName = "PingLogFiles\\Logfile";

        private static readonly string _defaultOutputTemplate = "[{Timestamp:dd-MM-yyyy HH:mm:ss}] {Message:lj}{NewLine}{Exception}";

        private static string _configFilePath;

        private static PingResults _pingResult;

        public static int PingTimeout => ((int)Math.Floor(_interval * 1000 * 0.8));

        private static IPAddress _target;
        private static int _interval;
        private static bool _optionsValid = true;

        static void Main(string[] args)
        {
            CommandLine.Parser.Default.ParseArguments<Options>(args)
                .WithParsed(RunOptions)
                .WithNotParsed(HandleParseError);

            var logfileName = ConfigurationManager.AppSettings["FileName"] ?? _defaultLogFileName;
            var outputTemplate = ConfigurationManager.AppSettings["OutputTemplate"] ?? _defaultOutputTemplate;

            if (_optionsValid)
            {
                _pingResult = new PingResults();


                var saveFile = Path.Combine(Directory.GetCurrentDirectory(), logfileName);
                saveFile = Path.ChangeExtension(saveFile, "txt");

                SetupLogger(outputTemplate, saveFile);

                LoggerTemplates.OutputStartText(_target, saveFile, _interval, PingTimeout);

                Log.Information("Pingvorgang gestartet.");

                while (!Console.KeyAvailable)
                {
                    PingHost(_target);
                    Thread.Sleep(_interval * 1000);
                }

                Console.ReadKey();

                LoggerTemplates.OutputStatistics(_pingResult);

                Log.CloseAndFlush();
            }

            Console.WriteLine("Um das Fenster zu schließen, drücke irgendeine Taste.");
            Console.ReadKey();
        }

        static void RunOptions(Options opts)
        {
            _target = IPAddress.Parse(opts.Target);
            _interval = opts.Intervall;
        }

        static void HandleParseError(IEnumerable<Error> errs)
        {
            _optionsValid = false;
        }

        private static void SetupLogger(string outputTemplate, string saveFile)
        {
            Log.Logger = new LoggerConfiguration()
                .MinimumLevel.Debug()
                .WriteTo.Console(outputTemplate: outputTemplate)
                .WriteTo.File(path: saveFile, restrictedToMinimumLevel: LogEventLevel.Information,
                    outputTemplate: outputTemplate, rollingInterval: RollingInterval.Day)
                .CreateLogger();
        }


        private static void CheckForConfigFile()
        {
            _configFilePath = "app.config";

            if (!File.Exists(_configFilePath))
            {
                System.Text.StringBuilder sb = new StringBuilder();
                sb.AppendLine("<?xml version=\"1.0\" encoding=\"utf-8\" ?>");
                sb.AppendLine("<configuration>");
                sb.AppendLine("<appSettings>");

                sb.AppendLine("<add key=\"FileName\" value=\"PingLogFiles\\Logfile\"/>");
                sb.AppendLine("<add key=\"OutputTemplate\" value=\"[{Timestamp:dd-MM-yyyy HH:mm:ss}] {Message:lj}{NewLine}{Exception}\"/>");
                sb.AppendLine("<add key=\"Target\" value=\"8.8.8.8\"/>");
                sb.AppendLine("</appSettings>");
                sb.AppendLine("</configuration>");

                File.WriteAllText(_configFilePath, sb.ToString());
            }
        }

        public static void PingHost(IPAddress ipAddress)
        {
            Ping pinger = null;

            try
            {
                pinger = new Ping();
                _pingResult.AmountOfTriedPings++;
                PingReply reply = pinger.Send(ipAddress, PingTimeout);
                _pingResult.AmountOfSendPings++;

                if (reply != null)
                {
                    Log.Information($"Pinged {reply.Address} Status: {reply.Status} Latency: {reply.RoundtripTime}ms");


                    if (reply.Status == IPStatus.Success)
                    {
                        _pingResult.SumOfLatency += reply.RoundtripTime;
                        _pingResult.MinLatency = Math.Min(_pingResult.MinLatency, reply.RoundtripTime);
                        _pingResult.MaxLatency = Math.Max(_pingResult.MaxLatency, reply.RoundtripTime);
                        _pingResult.AmountOfSuccess++;
                    }
                    else
                        _pingResult.AmountOfOther++;
                }
                else
                {
                    _pingResult.AmountOfOther++;
                }
            }
            catch (PingException pEx)
            {
                Log.Information($"Pinged {ipAddress} Status: Exception Message:{pEx.Message} => {pEx.InnerException?.Message ?? "No more detailed message"}");
                _pingResult.AmountOfExceptions++;
            }
            finally
            {
                pinger?.Dispose();
            }
        }

    }
}
