namespace BingoAPI.Helpers;

// TODO: Find a way to implement an universal solution without adding our own

/// <summary>
/// Class helping for logging stuff
/// </summary>
public static class Log
{
	/// <summary>
	/// Level of the log
	/// </summary>
	public enum LogLevel
	{
		/// <summary>
		/// Logs to help to debug
		/// </summary>
		Debug,

		/// <summary>
		/// Logs to inform about important steps
		/// </summary>
		Info,

		/// <summary>
		/// Logs to warn about an unwanted state
		/// </summary>
		Warning,

		/// <summary>
		/// Logs to notify of an error
		/// </summary>
		Error,
	}

	/// <summary>
	/// Callback to use for any log
	/// </summary>
	public static Action<LogLevel, string>? Logger { private get; set; }

	private static void LogMessage(LogLevel level, string? message) => Logger?.Invoke(level, message ?? string.Empty);

	/// <summary>
	/// Logs information for developers that helps to debug the mod
	/// </summary>
	internal static void Debug(string? message) => LogMessage(LogLevel.Debug, message);

	/// <summary>
	/// Logs information for players to know important steps of the mod
	/// </summary>
	internal static void Info(string? message) => LogMessage(LogLevel.Info, message);

	/// <summary>
	/// Logs information for players to warn them about an unwanted state
	/// </summary>
	internal static void Warning(string? message) => LogMessage(LogLevel.Warning, message);

	/// <summary>
	/// Logs information for players to notify them of an error
	/// </summary>
	internal static void Error(string? message) => LogMessage(LogLevel.Error, message);
}
