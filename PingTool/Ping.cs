using System;
using System.Net;
using System.Net.NetworkInformation;
using Serilog;

namespace PingTool;

internal class Ping
{
    public long Latency { get; init; }
    public required IPStatus Status { get; init; }
    public bool Exception { get; init; }
    public DateTimeOffset? Time { get; init; }

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
                Latency = pingResult.RoundtripTime,
                Status = pingResult.Status,
                Time = DateTimeOffset.UtcNow
            };
        }
        catch (PingException pEx)
        {
            Log.Information("Pinged {IpAddress} Status: Exception Message:{PExMessage} => {NoMoreDetailedMessage}",
                ipAddress, pEx.Message, pEx.InnerException?.Message ?? "No more detailed message");

            return new Ping
            {
                Exception = true,
                Status = IPStatus.Unknown
            };
        }
    }
}