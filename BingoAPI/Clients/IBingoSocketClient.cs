using System.Net.WebSockets;
using JetBrains.Annotations;

namespace BingoAPI.Clients;

/// <summary>
/// Interface that represents any class that handles websocket calls
/// </summary>
[PublicAPI]
public interface IBingoSocketClient : IDisposable
{
	/// <summary>
	/// Opens a <see cref="WebSocket"/> using the given key
	/// </summary>
	public Task Connect(string socketKey, Action<string> onMessageReceived, CancellationToken ct);

	/// <summary>
	/// Closes the <see cref="WebSocket"/> gracefully
	/// </summary>
	public Task Disconnect(CancellationToken ct);
}
