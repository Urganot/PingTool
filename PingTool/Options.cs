using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using CommandLine;

namespace PingTool;

internal class Options
{
    public List<ConsoleKey> CancelKeys = [ConsoleKey.Escape, ConsoleKey.X];

    [Option('t', "target", Default = new[] { "8.8.8.8", "192.168.178.1" }, HelpText = "Which IP should be tested?")]
    public IEnumerable<string> RawTargets
    {
        get { return Targets.Select(ip => ip.ToString()); }
        set => Targets = value.Select(IPAddress.Parse);
    }

    public IEnumerable<IPAddress> Targets { get; set; }

    [Option('i', "interval", Default = 1, HelpText = "Prints all messages to standard output.")]
    public int Interval { get; set; }

    [Option('c', "csv", Default = false, HelpText = "Creates an additional csv file.")]
    public bool OutputCsv { get; set; }

    [Option('f', "log-file", Default = "PingLogFiles\\Logfile", HelpText = "Name of the log file.")]
    public string LogFileName { get; set; }

    [Option('o', "output-template", Default = "[{Timestamp:dd-MM-yyyy HH:mm:ss}] {Message:lj}{NewLine}{Exception}",
        HelpText = "Template for the output text.")]
    public string OutputTemplate { get; set; }

    public int PingTimeout => (int)Math.Floor(Interval * 1000 * 0.8);
}