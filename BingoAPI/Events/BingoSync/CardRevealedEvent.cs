using BingoAPI.Models;
using Newtonsoft.Json;

namespace BingoAPI.Events.BingoSync;

/// <summary>
/// Event sent when a player reveals the card
/// </summary>
internal sealed record CardRevealedEvent : IEvent
{
	/// <summary>
	/// Player responsible for this event
	/// </summary>
	[JsonProperty("player")]
	[JsonRequired]
	public required Player Player { get; init; }
}
