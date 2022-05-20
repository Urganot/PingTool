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
        public List<Ping> pings = new List<Ping>();

        public void Add(Ping ping)
        {
            pings.Add(ping);
        }
        public int Count => pings.Count;

        public List<Ping> SuccessfulPings => pings.Where(ping => ping.Status == IPStatus.Success).ToList();
        public List<Ping> UnknownPings => pings.Where(ping => ping.Status == IPStatus.Unknown).ToList();
        public List<Ping> ExceptionPings => pings.Where(ping => ping.Exception).ToList();

        public List<Ping> SentPings => pings.Where(ping => !ping.Exception).ToList();

        public long MinLatency => SuccessfulPings.Min(ping => ping.Latency);
        public long MaxLatency => SuccessfulPings.Max(ping => ping.Latency);
        public double AvgLatency => SuccessfulPings.Average(ping => ping.Latency);
    }
}
