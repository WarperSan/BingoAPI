using BingoAPI.Logging;

namespace BingoAPI.Tests.TestObjects;

internal sealed class TestLogger : ILogger
{
	/// <inheritdoc />
	public void Debug(string message)
	{
		throw new NotImplementedException();
	}

	/// <inheritdoc />
	public void Info(string message)
	{
		throw new NotImplementedException();
	}

	/// <inheritdoc />
	public void Warning(string message)
	{
		throw new NotImplementedException();
	}

	/// <inheritdoc />
	public void Error(string message)
	{
		throw new NotImplementedException();
	}
}
