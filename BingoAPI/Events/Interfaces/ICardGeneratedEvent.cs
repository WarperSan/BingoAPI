using BingoAPI.Models;

namespace BingoAPI.Events.Interfaces;

/// <summary>
/// Represents any event sent when a player generates a new card
/// </summary>
public interface ICardGeneratedEvent : IEvent
{
	/// <summary>
	/// Player responsible for this event
	/// </summary>
	public Player Player { get; }

	/// <summary>
	/// Determines if the card was generated as hidden
	/// </summary>
	public bool IsCardHidden { get; }
}
