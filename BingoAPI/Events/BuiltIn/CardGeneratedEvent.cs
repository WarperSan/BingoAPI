using BingoAPI.Models;
using Newtonsoft.Json;

namespace BingoAPI.Events.BuiltIn;

/// <summary>
/// Event sent when a player generates a new card
/// </summary>
internal record CardGeneratedEvent : IEvent
{
	/// <summary>
	/// Player responsible for this event
	/// </summary>
	[JsonProperty("player")]
	[JsonRequired]
	public required Player Player { get; init; }

	/// <summary>
	/// Determines if the card was generated as hidden
	/// </summary>
	[JsonProperty("hide_card")]
	[JsonRequired]
	public required bool IsCardHidden { get; init; }

	/// <summary>
	/// Time when this event was sent
	/// </summary>
	[JsonProperty("timestamp")]
	[JsonRequired]
	public required ulong Timestamp { get; init; }
}
