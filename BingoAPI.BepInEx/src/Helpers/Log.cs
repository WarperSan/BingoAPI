using BepInEx.Logging;

namespace BingoAPI.BepInEx.Helpers;

/// <summary>
/// Class offering logging features
/// </summary>
internal static class Log
{
	private static readonly Lazy<ManualLogSource> Logger = new(() => global::BepInEx.Logging.Logger.CreateLogSource(Plugin.Id));

	/// <summary>
	/// Logs any message incoming from <see cref="BingoAPI"/> directly
	/// </summary>
	public static void LogCore(BingoAPI.Helpers.Log.LogLevel level, string message)
	{
		switch (level)
		{
			case BingoAPI.Helpers.Log.LogLevel.Debug:
				Debug(message);
				break;
			case BingoAPI.Helpers.Log.LogLevel.Warning:
				Warning(message);
				break;
			case BingoAPI.Helpers.Log.LogLevel.Error:
				Error(message);
				break;
			case BingoAPI.Helpers.Log.LogLevel.Info:
			default:
				Info(message);
				break;
		}
	}

	/// <summary>
	/// Logs information for developers that helps to debug the mod
	/// </summary>
	public static void Debug(string? message) => Logger.Value.LogDebug(message);

	/// <summary>
	/// Logs information for players to know important steps of the mod
	/// </summary>
	public static void Info(string? message) => Logger.Value.LogInfo(message);

	/// <summary>
	/// Logs information for players to warn them about an unwanted state
	/// </summary>
	public static void Warning(string? message) => Logger.Value.LogWarning(message);

	/// <summary>
	/// Logs information for players to notify them of an error
	/// </summary>
	public static void Error(string? message) => Logger.Value.LogError(message);
}
