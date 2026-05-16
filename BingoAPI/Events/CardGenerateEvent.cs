using BingoAPI.Models;
using Newtonsoft.Json;

namespace BingoAPI.Events;

/// <summary>
/// Event sent when a player generates a new card
/// </summary>
public record CardGenerateEvent : IBingoEvent
{
	/// <summary>
	/// Player responsible for this event
	/// </summary>
	[JsonProperty("player")]
	[JsonRequired]
	public Player Player = null!;

	/// <summary>
	/// Determines if the card was generated as hidden
	/// </summary>
	[JsonProperty("hide_card")]
	[JsonRequired]
	public bool IsCardHidden;

	/// <summary>
	/// Time when this event was sent
	/// </summary>
	[JsonProperty("timestamp")]
	[JsonRequired]
	public ulong Timestamp;
}
