namespace BingoAPI.Clients;

/// <summary>
/// Interface that represents any class that handles websocket calls
/// </summary>
public interface IBingoSocketClient : IDisposable
{
	/// <summary>
	/// Callback called when this client receives a message
	/// </summary>
	public event Action<string> OnMessageReceived;

	/// <summary>
	/// Opens a <see cref="WebSocket"/> using the given key
	/// </summary>
	public Task Connect(string socketKey, CancellationToken ct);

	/// <summary>
	/// Closes the <see cref="WebSocket"/> gracefully
	/// </summary>
	public Task Disconnect(CancellationToken ct);
}
