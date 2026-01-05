using System;
using System.IO;
using PingTool.Configuration;
using Serilog;
using Serilog.Events;

namespace PingTool.Logging;

internal static class LoggerUtils
{
    /// <summary>
    ///     Path of the log file currently in use by Serilog.
    ///     This is only set if a file sink is configured.
    /// </summary>
    public static string CurrentLogFilePath { get; private set; } = string.Empty;

    public static void SetupLogger(Options configuration)
    {
        var baseLogFilePath = GetBaseLogFilePath(configuration);

        var computedLogFilePath = ComputeLogFilePath(baseLogFilePath);

        CurrentLogFilePath = computedLogFilePath;

        if (configuration.OutputTemplate == null)
            throw new ArgumentNullException(nameof(configuration.OutputTemplate));

        Log.Logger = new LoggerConfiguration()
            .MinimumLevel.Debug()
            .WriteTo.Console(outputTemplate: configuration.OutputTemplate)
            .WriteTo.File(computedLogFilePath, LogEventLevel.Information, configuration.OutputTemplate)
            .CreateLogger();
    }

    private static string GetBaseLogFilePath(Options configuration)
    {
        string baseLogFilePath;

        if (configuration.LogFileName == null)
            throw new ArgumentNullException(nameof(configuration.LogFileName));

        if (Path.IsPathFullyQualified(configuration.LogFileName))
            baseLogFilePath = configuration.LogFileName;
        else
            baseLogFilePath = Path.Combine(Directory.GetCurrentDirectory(), configuration.LogFileName);

        baseLogFilePath = Path.ChangeExtension(baseLogFilePath, "txt");
        return baseLogFilePath;
    }

    private static string ComputeLogFilePath(string rawLogFilePath)
    {
        if (string.IsNullOrWhiteSpace(rawLogFilePath))
            return rawLogFilePath;

        var logFilePath = AppendDateString(rawLogFilePath);

        logFilePath = MakeUnique(logFilePath);

        return Path.ChangeExtension(logFilePath, "txt");
    }

    private static string MakeUnique(string startLogFilePath)
    {
        if (!Path.Exists(startLogFilePath))
            return startLogFilePath;

        var startLogFileNameNoExtension = Path.GetFileNameWithoutExtension(startLogFilePath);
        var suffix = 0;
        var logFilePath = startLogFilePath;

        do
        {
            suffix++;
            logFilePath = ChangeFileName(logFilePath, $"{startLogFileNameNoExtension}({suffix})");
        } while (Path.Exists(logFilePath));

        return logFilePath;
    }

    private static string ChangeFileName(string path, string newFileName)
    {
        var oldExtension = Path.GetExtension(path);
        var directory = Path.GetDirectoryName(path) ?? string.Empty;

        var newFileNameNoExtension = Path.GetFileNameWithoutExtension(newFileName);
        var newExtension = Path.GetExtension(newFileName);

        var newPathNoExtension = Path.Combine(directory, newFileNameNoExtension);

        var newPath = Path.ChangeExtension(
            newPathNoExtension,
            string.IsNullOrWhiteSpace(newExtension) ? oldExtension : newExtension
        );

        return newPath;
    }

    private static string AppendDateString(
        string logFilePath,
        string dateFormat = "yyyy-MM-dd",
        DateTime? date = null,
        string delimiter = "_"
    )
    {
        date ??= DateTime.Now;

        var fileName = Path.GetFileNameWithoutExtension(logFilePath);
        var fileNameWithDate = fileName + delimiter + date.Value.ToString(dateFormat);

        return ChangeFileName(logFilePath, fileNameWithDate);
    }
}