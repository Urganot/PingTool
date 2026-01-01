namespace PingTool;

public static class Defaults
{
    public const string DefaultTarget = "8.8.8.8";
    public const int DefaultIntervall = 1;

    public const string DefaultLogFileName = "PingLogFiles\\Logfile";

    public const string DefaultOutputTemplate = "[{Timestamp:dd-MM-yyyy HH:mm:ss}] {Message:lj}{NewLine}{Exception}";
}