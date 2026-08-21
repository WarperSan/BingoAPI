using System.Net.WebSockets;

namespace BingoAPI.Network.System;

/// <summary>
/// Default implementation of <see cref="IWebSocketProvider"/> for <see cref="System"/>
/// </summary>
public sealed class SystemWebSocketProvider : IWebSocketProvider
{
	/// <inheritdoc />
	public ClientWebSocket CreateClient()
	{
		return new ClientWebSocket();
	}
}
