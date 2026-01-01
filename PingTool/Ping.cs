using System;
using System.Net;
using System.Net.NetworkInformation;
using Serilog;

namespace PingTool;

internal class Ping
{
    public string Target { get; set; }
    public long Latency { get; set; }
    public IPStatus Status { get; set; }
    public bool Exception { get; set; }
    public string Time { get; set; }

    public static Ping Send(IPAddress ipAddress, int pingTimeout)
    {
        try
        {
            var pingResult = new System.Net.NetworkInformation.Ping().Send(ipAddress, pingTimeout);

            if (pingResult != null)
            {
                Log.Information(
                    $"Pinged {pingResult.Address} Status: {pingResult.Status} Latency: {pingResult.RoundtripTime}ms");
                return new Ping
                {
                    Target = pingResult.Address.ToString(),
                    Latency = pingResult.RoundtripTime,
                    Status = pingResult.Status,
                    Time = DateTime.Now.ToString("dd-MM-yyyy HH:mm:ss")
                };
            }

            return new Ping
            {
                Target = ipAddress.ToString(),
                Status = IPStatus.Unknown
            };
        }
        catch (PingException pEx)
        {
            Log.Information(
                $"Pinged {ipAddress} Status: Exception Message:{pEx.Message} => {pEx.InnerException?.Message ?? "No more detailed message"}");
            return new Ping
            {
                Exception = true
            };
        }
    }
}