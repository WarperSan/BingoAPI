using System.Net.WebSockets;

namespace BingoAPI.Network;

/// <summary>
/// Interface that represents any class that can provide <see cref="WebSocket"/> instances
/// </summary>
public interface IWebSocketProvider
{
	/// <summary>
	/// Creates a new instance of <see cref="ClientWebSocket"/>
	/// </summary>
	public ClientWebSocket CreateClient();
}
