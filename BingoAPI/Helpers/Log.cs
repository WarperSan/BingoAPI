using BepInEx.Logging;

namespace BingoAPI.Helpers;

/// <summary>
///     Class helping for logging stuff
/// </summary>
internal static class Log
{
	private static ManualLogSource? _logger;

	private static void LogSelf(string message, LogLevel level)
	{
		_logger ??= Logger.CreateLogSource(Plugin.Id);
		_logger.Log(level, message);
	}

	/// <summary>
	///     Logs information for developers that helps to debug the mod
	/// </summary>
	public static void Debug(string message) => LogSelf(message, LogLevel.Debug);

	/// <summary>
	///     Logs information for players to know important steps of the mod
	/// </summary>
	public static void Info(string message) => LogSelf(message, LogLevel.Message);

	/// <summary>
	///     Logs information for players to warn them about an unwanted state
	/// </summary>
	public static void Warning(string message) => LogSelf(message, LogLevel.Warning);

	/// <summary>
	///     Logs information for players to notify them of an error
	/// </summary>
	public static void Error(string message) => LogSelf(message, LogLevel.Error);
}
