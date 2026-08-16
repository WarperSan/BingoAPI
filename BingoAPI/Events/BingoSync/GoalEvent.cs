using BingoAPI.Models;

namespace BingoAPI.Events.BingoSync;

/// <summary>
/// Event sent when a player marks or clears a square
/// </summary>
internal record GoalEvent : IEvent
{
	/// <summary>
	/// Player responsible for this event
	/// </summary>
	public required Player Player { get; init; }

	/// <summary>
	/// Square modified by this event
	/// </summary>
	public required Square Square { get; init; }

	/// <summary>
	/// Team that was added or removed
	/// </summary>
	public required Team Team { get; init; }

	/// <summary>
	/// Defines if the selected square has been cleared or marked
	/// </summary>
	public required bool HasBeenCleared { get; init; }
}
