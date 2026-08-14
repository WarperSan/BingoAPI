using BingoAPI.Goals;
using BingoAPI.Models;
using BingoAPI.Models.Settings;

namespace BingoAPI.Clients.Interfaces;

/// <summary>
/// Interface that represents any class that represents an active connection to the server
/// </summary>
public interface IBingoSessionClient : IDisposable
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
	/// Creates a room and joins it
	/// </summary>
	public Task<bool> CreateRoom(CreateRoomSettings settings, CancellationToken ct = default);

	/// <summary>
	/// Joins the room
	/// </summary>
	public Task<bool> JoinRoom(JoinRoomSettings settings, CancellationToken ct = default);

	/// <summary>
	/// Leaves the room
	/// </summary>
	public Task<bool> LeaveRoom(CancellationToken ct = default);

	/// <summary>
	/// Sends a message in the room
	/// </summary>
	public Task<bool> SendMessage(string message, CancellationToken ct = default);

	/// <summary>
	/// Changes the player's team
	/// </summary>
	public Task<bool> ChangeTeam(Team team, CancellationToken ct = default);

	/// <summary>
	/// Gets the current <see cref="Card"/> of the room
	/// </summary>
	public Task<Card?> GetCard(GoalPool pool, CancellationToken ct = default);

	/// <summary>
	/// Marks the square for a team
	/// </summary>
	public Task<bool> MarkSquare(int index, CancellationToken ct = default);

	/// <summary>
	/// Clears the square for a team
	/// </summary>
	public Task<bool> ClearSquare(int index, CancellationToken ct = default);

	/// <summary>
	/// Reveals the card for the room
	/// </summary>
	public Task<bool> RevealCard(CancellationToken ct = default);
}
