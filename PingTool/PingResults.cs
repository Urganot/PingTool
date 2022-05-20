using System.Collections.Generic;
using System.Text;
using static System.Int64;

namespace PingTool
{
    class PingResults
    {
        public float AmountOfTriedPings = 0;
        public float AmountOfSendPings = 0;
        public float AmountOfSuccess = 0;
        public float AmountOfOther = 0;

        public long MinLatency { get; set; } = MaxValue;
        public long MaxLatency { get; set; } = 0;
        public long SumOfLatency = 0;


        public int AmountOfExceptions { get; set; }

        public float AmountOfSendPingsPercent => AmountOfSendPings / AmountOfTriedPings * 100;
        public float AmountOfSuccessfulPingsPercent => AmountOfSuccess / AmountOfSendPings * 100;
        public float AmountOfOtherPingsPercent => AmountOfOther / AmountOfSendPings * 100;
        public float AmountOfExceptionsPercent => AmountOfExceptions / AmountOfTriedPings * 100;
        public float AverageLatency => SumOfLatency / AmountOfSuccess;
    }
}
