using BingoAPI.Converters;
using BingoAPI.Models;
using Newtonsoft.Json;

namespace BingoAPI.Events.BingoSync;

/// <summary>
/// Event sent when a player joins or leaves the room
/// </summary>
internal sealed record ConnectionEvent : IEvent
{
	/// <summary>
	/// Player responsible for this event
	/// </summary>
	[JsonProperty("player")]
	[JsonRequired]
	public required Player Player { get; init; }

	/// <summary>
	/// Defines if the player has connected or disconnected
	/// </summary>
	[JsonProperty("event_type")]
	[JsonConverter(typeof(ObjectEqualConverter), "connected")]
	public required bool IsConnected { get; init; }
}
