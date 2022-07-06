using System.Collections.Generic;
using System.Net;
using CommandLine;

namespace PingTool
{
    internal static class CommandLineParser
    {
        public static void RunOptions(Options opts)
        {
            PingTool.Target = IPAddress.Parse(opts.Target);
            PingTool.Interval = opts.Interval;
            PingTool.OutPutCsv = opts.OutputCsv;
        }

        public static void HandleParseError(IEnumerable<Error> errs)
        {
            PingTool.OptionsValid = false;
        }
    }
}