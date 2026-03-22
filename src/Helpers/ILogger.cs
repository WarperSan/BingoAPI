namespace BingoAPI.Helpers;

/// <summary>
/// Interface used to represent any class that can be used in <see cref="Helpers.Log"/>
/// </summary>
public interface ILogger
{
	/// <summary>
	/// Logs the given message with the specified severity level
	/// </summary>
	void Log(string? message, LogLevel level);
}
