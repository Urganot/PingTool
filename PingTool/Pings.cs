using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.NetworkInformation;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace PingTool
{
    class Pings
    {
        public List<Ping> pings = new();

        public void Add(Ping ping)
        {
            pings.Add(ping);
        }
        public int Count => pings.Count;

        public IEnumerable<Ping> SuccessfulPings => pings.Where(ping => ping.Status == IPStatus.Success).ToList();
        public double SuccessfulPingsPercentage => ((double)SuccessfulPings.Count() / pings.Count) * 100;
        public IEnumerable<Ping> FailedPings => pings.Where(ping => ping.Status != IPStatus.Success);//.ToList();
        public double FailedPingsPercent => ((double)FailedPings.Count() / pings.Count) * 100;
        public IEnumerable<Ping> ExceptionPings => pings.Where(ping => ping.Exception).ToList();
        public double ExceptionPingsPercentage => ((double)ExceptionPings.Count() / pings.Count) * 100;

        public IEnumerable<Ping> SentPings => pings.Where(ping => !ping.Exception).ToList();

        public long MinLatency => SuccessfulPings.Min(ping => ping.Latency);
        public long MaxLatency => SuccessfulPings.Max(ping => ping.Latency);
        public double AvgLatency => SuccessfulPings.Average(ping => ping.Latency);
    }
}
