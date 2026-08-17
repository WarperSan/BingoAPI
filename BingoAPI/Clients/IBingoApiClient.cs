using System.Net.WebSockets;
using BingoAPI.Configuration.Settings;
using BingoAPI.Models;
using JetBrains.Annotations;

namespace BingoAPI.Clients;

/// <summary>
/// Interface that represents any class that handles HTTP calls to the server
/// </summary>
[PublicAPI]
public interface IBingoApiClient
{
	// TODO: Replace String with type appropriate replacements (RoomCode, SocketKey, etc)

	/// <summary>
	/// Creates a room with the given settings
	/// </summary>
	/// <returns>Code of the room</returns>
	public Task<string> CreateRoom(CreateRoomSettings settings, CancellationToken ct);

	/// <summary>
	/// Joins the room with the given settings
	/// </summary>
	/// <returns>
	/// Socket key of the <see cref="WebSocket"/>
	/// </returns>
	public Task<string> JoinRoom(JoinRoomSettings settings, CancellationToken ct);

	/// <summary>
	/// Marks the square at the given index for a certain team
	/// </summary>
	public Task MarkSquare(string room, Team team, int index, CancellationToken ct);

	/// <summary>
	/// Clears the square at the given index for a certain team
	/// </summary>
	public Task ClearSquare(string room, Team team, int index, CancellationToken ct);

	/// <summary>
	/// Sends a message in the room
	/// </summary>
	public Task SendMessage(string room, string message, CancellationToken ct);

	/// <summary>
	/// Changes the team of the client in the room
	/// </summary>
	public Task ChangeTeam(string room, Team team, CancellationToken ct);

	/// <summary>
	/// Gets all the squares of the room
	/// </summary>
	public Task<ICollection<Square>> GetSquares(string room, CancellationToken ct);

	/// <summary>
	/// Reveals the card in the room
	/// </summary>
	public Task RevealCard(string room, CancellationToken ct);

	// TODO: public Task NewCard(string room, CancellationToken ct);

	/// <summary>
	/// Gets the identity of the socket related to the given key
	/// </summary>
	public Task<SocketIdentity> GetSocketIdentity(string socketKey, CancellationToken ct);
}
