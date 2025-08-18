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

    /// <summary>
    /// Logs information for developers that helps to debug the mod
    /// </summary>
    public static void Debug(object? data) => LogSelf(data, LogLevel.Debug);

    /// <summary>
    /// Logs information for players to know important steps of the mod
    /// </summary>
    public static void Info(object? data) => LogSelf(data, LogLevel.Message);
    
    /// <summary>
    /// Logs information for players to warn them about an unwanted state
    /// </summary>
    public static void Warning(object? data) => LogSelf(data, LogLevel.Warning);
    
    /// <summary>
    /// Logs information for players to notify them of an error
    /// </summary>
    public static void Error(object? data) => LogSelf(data, LogLevel.Error);
}