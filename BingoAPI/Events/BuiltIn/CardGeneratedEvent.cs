using BingoAPI.Models;
using Newtonsoft.Json;

namespace BingoAPI.Events.BuiltIn;

/// <summary>
/// Event sent when a player generates a new card
/// </summary>
internal record CardGeneratedEvent : IBingoEvent
{
	/// <summary>
	/// Player responsible for this event
	/// </summary>
	[JsonProperty("player")]
	[JsonRequired]
	public readonly Player Player = null!;

	/// <summary>
	/// Determines if the card was generated as hidden
	/// </summary>
	[JsonProperty("hide_card")]
	[JsonRequired]
	public readonly bool IsCardHidden;

	/// <summary>
	/// Time when this event was sent
	/// </summary>
	[JsonProperty("timestamp")]
	[JsonRequired]
	public readonly ulong Timestamp;
}
