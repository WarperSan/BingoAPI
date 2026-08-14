using BingoAPI.Models;

namespace BingoAPI.Events.Interfaces;

/// <summary>
/// Represents any event sent when a player marks or clears a square
/// </summary>
public interface IGoalEvent : IEvent
{
	/// <summary>
	/// Player responsible for this event
	/// </summary>
	public Player Player { get; }

	/// <summary>
	/// Square modified by this event
	/// </summary>
	public Square Square { get; }

	/// <summary>
	/// Team that was added or removed
	/// </summary>
	public Team Team { get; }

	/// <summary>
	/// Defines if the selected square has been cleared or marked
	/// </summary>
	public bool HasBeenCleared { get; }
}
