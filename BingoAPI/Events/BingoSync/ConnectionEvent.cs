using BingoAPI.Models;

namespace BingoAPI.Events.BingoSync;

/// <summary>
/// Event sent when a player joins or leaves the room
/// </summary>
internal record ConnectionEvent : IEvent
{
	/// <summary>
	/// Player responsible for this event
	/// </summary>
	public required Player Player { get; init; }

	/// <summary>
	/// Defines if the player has connected or disconnected
	/// </summary>
	public required bool IsConnected { get; init; }
}
