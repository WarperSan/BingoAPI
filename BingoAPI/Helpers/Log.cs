using UnityEngine;

namespace BingoAPI.Helpers;

/// <summary>
/// Class helping for logging stuff
/// </summary>
public static class Log
{
	private static Action<string>? _logger;

	/// <summary>
	/// Sets the <see cref="ILogger"/> to use
	/// </summary>
	public static void SetLogger(Action<string> logger) => _logger = logger;

	/// <summary>
	/// Logs information for developers that helps to debug the mod
	/// </summary>
	internal static void Debug(string? message) => _logger?.Invoke(message);

	/// <summary>
	/// Logs information for players to know important steps of the mod
	/// </summary>
	internal static void Info(string? message) => _logger?.Invoke(message);

	/// <summary>
	/// Logs information for players to warn them about an unwanted state
	/// </summary>
	internal static void Warning(string? message) => _logger?.Invoke(message);

	/// <summary>
	/// Logs information for players to notify them of an error
	/// </summary>
	internal static void Error(string? message) => _logger?.Invoke(message);
}
