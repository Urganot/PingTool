using System.Collections.Generic;
using CommandLine;
using PingTool;

class Options
{
    [Option('t', "target", Default = Defaults.DefaultTarget, HelpText = "Which IP should be tested?")]
    public string Target { get; set; }

    [Option('i', "intervall", Default = Defaults.DefaultIntervall, HelpText = "Prints all messages to standard output.")]
    public int Intervall { get; set; }

}
