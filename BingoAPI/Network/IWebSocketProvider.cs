using System.Net.WebSockets;
using JetBrains.Annotations;

namespace BingoAPI.Network;

/// <summary>
/// Interface that represents any class that can provide <see cref="WebSocket"/> instances
/// </summary>
[PublicAPI]
public interface IWebSocketProvider
{
	/// <summary>
	/// Creates a new instance of <see cref="ClientWebSocket"/>
	/// </summary>
	public ClientWebSocket CreateClient();
}
