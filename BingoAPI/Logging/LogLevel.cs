using JetBrains.Annotations;

namespace BingoAPI.Logging;

/// <summary>
/// Level of the log
/// </summary>
[PublicAPI]
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
