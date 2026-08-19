namespace BingoAPI.Logging;

/// <summary>
/// Interface that represents any class that can log messages
/// </summary>
public interface ILogger
{
	/// <summary>
	/// Logs information for developers that helps to debug the mod
	/// </summary>
	public void Debug(string message);

	/// <summary>
	/// Logs information for players to know important steps of the mod
	/// </summary>
	public void Info(string message);

	/// <summary>
	/// Logs information for players to warn them about an unwanted state
	/// </summary>
	public void Warning(string message);

	/// <summary>
	/// Logs information for players to notify them of an error
	/// </summary>
	public void Error(string message);
}
