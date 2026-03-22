using Microsoft.Extensions.Logging;

namespace BingoAPI.Helpers;

/// <summary>
/// Class helping for logging stuff
/// </summary>
public static class Log
{
	/// <summary>
	/// Instance of <see cref="ILogger"/> to use
	/// </summary>
	public static ILogger? Logger
	{
		private get;
		set;
	}

	/// <summary>
	/// Logs information for developers that helps to debug the mod
	/// </summary>
	internal static void Debug(string? message) => Logger?.LogDebug("{Message}", message);

	/// <summary>
	/// Logs information for players to know important steps of the mod
	/// </summary>
	internal static void Info(string? message) => Logger?.LogInformation("{Message}", message);

	/// <summary>
	/// Logs information for players to warn them about an unwanted state
	/// </summary>
	internal static void Warning(string? message) => Logger?.LogWarning("{Message}", message);

	/// <summary>
	/// Logs information for players to notify them of an error
	/// </summary>
	internal static void Error(string? message) => Logger?.LogError("{Message}", message);
}
