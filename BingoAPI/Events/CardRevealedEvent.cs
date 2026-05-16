using BingoAPI.Models;
using Newtonsoft.Json;

namespace BingoAPI.Events;

/// <summary>
/// Event sent when a player reveals the card
/// </summary>
public record CardRevealedEvent : IBingoEvent
{
	/// <summary>
	/// Player responsible for this event
	/// </summary>
	[JsonProperty("player")]
	[JsonRequired]
	public Player Player = null!;

	/// <summary>
	/// Time when this event was sent
	/// </summary>
	[JsonProperty("timestamp")]
	[JsonRequired]
	public ulong Timestamp;
}
