namespace BingoAPI.Helpers;

/// <summary>
/// Defines log severity levels
/// </summary>
public enum LogLevel : byte
{
	Debug,
	Info,
	Warning,
	Error
}

/// <summary>
/// Class helping for logging stuff
/// </summary>
public static class Log
{
	private static ILogger? _logger;

	/// <summary>
	/// Sets the <see cref="ILogger"/> to use
	/// </summary>
	public static void SetLogger(ILogger logger) => _logger = logger;

	/// <summary>
	/// Logs information for developers that helps to debug the mod
	/// </summary>
	internal static void Debug(string? message) => _logger?.Log(message, LogLevel.Debug);

	/// <summary>
	/// Logs information for players to know important steps of the mod
	/// </summary>
	internal static void Info(string? message) => _logger?.Log(message, LogLevel.Info);

	/// <summary>
	/// Logs information for players to warn them about an unwanted state
	/// </summary>
	internal static void Warning(string? message) => _logger?.Log(message, LogLevel.Warning);

	/// <summary>
	/// Logs information for players to notify them of an error
	/// </summary>
	internal static void Error(string? message) => _logger?.Log(message, LogLevel.Error);
}
