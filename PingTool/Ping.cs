using System;
using System.Net;
using System.Net.NetworkInformation;
using Serilog;

namespace PingTool;

internal class Ping
{
    public required string Target { get; init; }
    public long Latency { get; init; }
    public required IPStatus Status { get; init; }
    public bool Exception { get; init; }
    public string? Time { get; init; }

    public static Ping Send(IPAddress ipAddress, int pingTimeout)
    {
        try
        {
            var pingResult = new System.Net.NetworkInformation.Ping().Send(ipAddress, pingTimeout);

            Log.Information(
                "Pinged {PingResultAddress} Status: {PingResultStatus} Latency: {PingResultRoundtripTime}ms",
                pingResult.Address, pingResult.Status, pingResult.RoundtripTime);
            return new Ping
            {
                Target = pingResult.Address.ToString(),
                Latency = pingResult.RoundtripTime,
                Status = pingResult.Status,
                Time = DateTime.Now.ToString("dd-MM-yyyy HH:mm:ss")
            };
        }
        catch (PingException pEx)
        {
            Log.Information("Pinged {IpAddress} Status: Exception Message:{PExMessage} => {NoMoreDetailedMessage}",
                ipAddress, pEx.Message, pEx.InnerException?.Message ?? "No more detailed message");

            return new Ping
            {
                Target = ipAddress.ToString(),
                Exception = true,
                Status = IPStatus.Unknown
            };
        }
    }
}