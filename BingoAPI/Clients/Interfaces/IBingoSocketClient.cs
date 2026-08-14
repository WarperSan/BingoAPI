using System.Net.WebSockets;

namespace BingoAPI.Clients.Interfaces;

/// <summary>
/// Interface that represents any class that handles websocket calls to the server
/// </summary>
public interface IBingoSocketClient : IDisposable
{
	/// <summary>
	/// Opens a <see cref="WebSocket"/> using the given key
	/// </summary>
	public Task Connect(string socketKey, CancellationToken ct);

	/// <summary>
	/// Closes the <see cref="WebSocket"/> gracefully
	/// </summary>
	public Task Disconnect(CancellationToken ct);
}
