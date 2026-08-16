using BingoAPI.Models;

namespace BingoAPI.Events.BingoSync;

/// <summary>
/// Event sent when a player generates a new card
/// </summary>
internal record CardGeneratedEvent : IEvent
{
	/// <summary>
	/// Player responsible for this event
	/// </summary>
	public required Player Player { get; init; }

	/// <summary>
	/// Determines if the card was generated as hidden
	/// </summary>
	public required bool IsCardHidden { get; init; }
}
