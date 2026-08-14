using BingoAPI.Models;

namespace BingoAPI.Events.Interfaces;

/// <summary>
/// Represents any event sent when a player reveals the card
/// </summary>
public interface ICardRevealedEvent : IEvent
{
	/// <summary>
	/// Player responsible for this event
	/// </summary>
	public Player Player { get; }
}
