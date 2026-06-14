using BingoAPI.Models;
using Newtonsoft.Json;

namespace BingoAPI.Events.BuiltIn;

/// <summary>
/// Event sent when a player reveals the card
/// </summary>
internal record CardRevealedEvent : IEvent
{
	/// <summary>
	/// Player responsible for this event
	/// </summary>
	[JsonProperty("player")]
	[JsonRequired]
	public required Player Player { get; init; }

	/// <summary>
	/// Time when this event was sent
	/// </summary>
	[JsonProperty("timestamp")]
	[JsonRequired]
	public required ulong Timestamp { get; init; }
}
