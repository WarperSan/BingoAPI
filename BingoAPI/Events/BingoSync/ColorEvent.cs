using BingoAPI.Models;

namespace BingoAPI.Events.BingoSync;

/// <summary>
/// Event sent when a player changes team
/// </summary>
internal record ColorEvent : IEvent
{
	/// <summary>
	/// Player responsible for this event
	/// </summary>
	public required Player Player { get; init; }

	/// <summary>
	/// New color of the player
	/// </summary>
	public required Team NewColor { get; init; }
}
