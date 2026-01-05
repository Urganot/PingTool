using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.NetworkInformation;
using PingTool.Logging;

namespace PingTool.Pings;

internal class PingResult
{
    public readonly List<Ping> ListOfPings = [];
    public required IPAddress Target { get; init; }

    public int Count => ListOfPings.Count;

    public IEnumerable<Ping> SentPings => ListOfPings.Where(ping => !ping.Exception).ToList();
    public double SentPingsPercentage => (double)SentPings.Count() / ListOfPings.Count;

    public IEnumerable<Ping> ExceptionPings => ListOfPings.Where(ping => ping.Exception).ToList();
    public double ExceptionPingsPercentage => (double)ExceptionPings.Count() / ListOfPings.Count;

    public IEnumerable<Ping> SuccessfulPings => ListOfPings.Where(ping => ping.Status == IPStatus.Success).ToList();
    public double SuccessfulPingsPercentage => (double)SuccessfulPings.Count() / ListOfPings.Count;

    public IEnumerable<Ping> FailedPings => ListOfPings.Where(ping => ping.Status != IPStatus.Success);
    public double FailedPingsPercent => (double)FailedPings.Count() / ListOfPings.Count;


    public long MinLatency => SuccessfulPings.Min(ping => ping.Latency);
    public long MaxLatency => SuccessfulPings.Max(ping => ping.Latency);
    public double AvgLatency => SuccessfulPings.Average(ping => ping.Latency);

    public void Add(Ping ping)
    {
        ListOfPings.Add(ping);
    }

    public void OutputIntermediateStatistics()
    {
        LoggerTemplates.OutputIntermediateStatistics(this);
    }

    public void OutputEndStatistics()
    {
        LoggerTemplates.OutputEndStatistics(this);
    }
}