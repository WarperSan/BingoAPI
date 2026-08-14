using BingoAPI.Models;

namespace BingoAPI.Events.Interfaces;

/// <summary>
/// Represents any event sent when a player changes team
/// </summary>
public interface IColorEvent : IEvent
{
	/// <summary>
	/// Player responsible for this event
	/// </summary>
	public Player Player { get; }

	/// <summary>
	/// New color of the player
	/// </summary>
	public Team NewColor { get; }
}
