using System.Net.WebSockets;
using JetBrains.Annotations;

namespace BingoAPI.Models;

/// <summary>
/// Represents an identity for a <see cref="WebSocket"/>
/// </summary>
[PublicAPI]
public record SocketIdentity
{
	/// <summary>
	/// Code of the room the socket connects to
	/// </summary>
	public required string Code { get; init; }

	/// <summary>
	/// Unique identifier of the player that created the socket
	/// </summary>
	public required string PlayerUUID { get; init; }
}
