using BingoAPI.Configuration.Settings;
using BingoAPI.Goals;
using BingoAPI.Models;
using JetBrains.Annotations;

namespace BingoAPI.Clients;

/// <summary>
/// Interface that represents any class that represents an active connection to the server
/// </summary>
[PublicAPI]
public interface IBingoSessionClient
{
	/// <summary>
	/// Team of the player
	/// </summary>
	public Team Team { get; }

	/// <summary>
	/// Defines if this instance is connected to a room
	/// </summary>
	public bool IsInRoom { get; }

	/// <summary>
	/// Creates a new room with the given settings
	/// </summary>
	public Task<bool> CreateRoom(CreateRoomSettings settings, CancellationToken ct = default);

	/// <summary>
	/// Joins an existing room with the given settings
	/// </summary>
	public Task<bool> JoinRoom(JoinRoomSettings settings, CancellationToken ct = default);

	/// <summary>
	/// Leaves the current room
	/// </summary>
	public Task<bool> LeaveRoom(CancellationToken ct = default);

	/// <summary>
	/// Sends a message in the current room
	/// </summary>
	public Task<bool> SendMessage(string message, CancellationToken ct = default);

	/// <summary>
	/// Changes the current player's team
	/// </summary>
	public Task<bool> ChangeTeam(Team team, CancellationToken ct = default);

	/// <summary>
	/// Gets the current <see cref="Card"/> of the current room
	/// </summary>
	public Task<Card?> GetCard(IGoalPool pool, CancellationToken ct = default);

	/// <summary>
	/// Marks the square at the given index for the current player
	/// </summary>
	public Task<bool> MarkSquare(int index, CancellationToken ct = default);

	/// <summary>
	/// Clears the square at the given index for the current player
	/// </summary>
	public Task<bool> ClearSquare(int index, CancellationToken ct = default);

	/// <summary>
	/// Reveals the card for the current room
	/// </summary>
	public Task<bool> RevealCard(CancellationToken ct = default);
}
