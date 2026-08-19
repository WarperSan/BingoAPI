namespace BingoAPI.Logging;

/// <summary>
/// Class used for sending log messages to a given callback
/// </summary>
public class Logger : ILogger
{
	private readonly Action<LogLevel, string> _onLog;

	/// <summary>
	/// Initializes a new instance of the <see cref="Logger"/> class.
	/// </summary>
	public Logger(Action<LogLevel, string> onLog)
	{
		_onLog = onLog;
	}

	/// <summary>
	/// Logs the given message under the given <see cref="LogLevel"/>
	/// </summary>
	private void Log(LogLevel level, string message)
	{
		_onLog.Invoke(level, message);
	}

	/// <inheritdoc />
	public void Debug(string message)
	{
		Log(LogLevel.Debug, message);
	}

	/// <inheritdoc />
	public void Info(string message)
	{
		Log(LogLevel.Info, message);
	}

	/// <inheritdoc />
	public void Warning(string message)
	{
		Log(LogLevel.Warning, message);
	}

	/// <inheritdoc />
	public void Error(string message)
	{
		Log(LogLevel.Error, message);
	}
}
