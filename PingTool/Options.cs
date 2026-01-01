using System.Collections.Generic;
using CommandLine;
using PingTool;

class Options
{
    [Option('t', "target", Default = Defaults.DefaultTarget, HelpText = "Which IP should be tested?")]
    public string Target { get; set; }

    [Option('i', "interval", Default = Defaults.DefaultIntervall, HelpText = "Prints all messages to standard output.")]
    public int Interval { get; set; }

    [Option('c', "csv", Default = Defaults.DefaultIntervall, HelpText = "Creates an additional csv file.")]
    public bool OutputCsv { get; set; }
    
    [Option('f', "log-file", Default = Defaults.DefaultLogFileName, HelpText = "Name of the log file.")]
    public string LogFileName { get; set; }
    
    [Option('o', "output-template", Default = Defaults.DefaultOutputTemplate, HelpText = "Template for the output text.")]
    public string OutputTemplate { get; set; }
}
