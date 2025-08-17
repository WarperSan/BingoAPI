using BepInEx.Logging;

namespace BingoAPI.Helpers;

/// <summary>
/// Class helping for logging stuff
/// </summary>
internal static class Logger
{
    private static ManualLogSource? _logger;

    /// <summary>
    /// Assigns the current logger to the given logger
    /// </summary>
    public static void SetLogger(ManualLogSource? logger) => _logger = logger;

    private static void LogSelf(object? data, LogLevel level)
    {
        var message = data ?? "null";

        _logger?.Log(level, message.ToString());
#if DEBUG
        UnityEngine.Debug.Log($"[{level}] {message}");
        System.Console.WriteLine($"[{level}] {message}");
#endif
    }

    /// <inheritdoc cref="BepInEx.Logging.ManualLogSource.LogDebug"/>
    public static void Debug(object? data) => LogSelf(data, LogLevel.Debug);
    
    /// <inheritdoc cref="BepInEx.Logging.ManualLogSource.LogInfo"/>
    public static void Info(object? data) => LogSelf(data, LogLevel.Message);
    
    /// <inheritdoc cref="BepInEx.Logging.ManualLogSource.LogWarning"/>
    public static void Warning(object? data) => LogSelf(data, LogLevel.Warning);
    
    /// <inheritdoc cref="BepInEx.Logging.ManualLogSource.LogError"/>
    public static void Error(object? data) => LogSelf(data, LogLevel.Error);
}