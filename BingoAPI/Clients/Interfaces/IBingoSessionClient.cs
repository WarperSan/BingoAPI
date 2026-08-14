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
	/// Creates a room and joins it
	/// </summary>
	public Task<bool> CreateRoom(CreateRoomSettings settings, CancellationToken ct);

	/// <summary>
	/// Joins the room
	/// </summary>
	public Task<bool> JoinRoom(JoinRoomSettings settings, CancellationToken ct);

	/// <summary>
	/// Leaves the room
	/// </summary>
	public Task<bool> LeaveRoom(CancellationToken ct);

	/// <summary>
	/// Sends a message in the room
	/// </summary>
	public Task<bool> SendMessage(string message, CancellationToken ct);

	/// <summary>
	/// Changes the player's team
	/// </summary>
	public Task<bool> ChangeTeam(Team team, CancellationToken ct);

	/// <summary>
	/// Gets the current <see cref="Card"/> of the room
	/// </summary>
	public Task<Card?> GetCard(GoalPool pool, CancellationToken ct);

	/// <summary>
	/// Marks the square for a team
	/// </summary>
	public Task<bool> MarkSquare(int index, CancellationToken ct);

	/// <summary>
	/// Clears the square for a team
	/// </summary>
	public Task<bool> ClearSquare(int index, CancellationToken ct);

	/// <summary>
	/// Reveals the card for the room
	/// </summary>
	public Task<bool> RevealCard(CancellationToken ct);
}
