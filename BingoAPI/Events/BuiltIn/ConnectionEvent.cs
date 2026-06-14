using BingoAPI.Models;
using BingoAPI.Networking.Converters;
using Newtonsoft.Json;

namespace BingoAPI.Events.BuiltIn;

/// <summary>
/// Event sent when a player joins or leaves the room
/// </summary>
internal record ConnectionEvent : IEvent
{
	/// <summary>
	/// Player responsible for this event
	/// </summary>
	[JsonProperty("player")]
	[JsonRequired]
	public required Player Player { get; init; }

	/// <summary>
	/// Identifier of the room
	/// </summary>
	[JsonProperty("room")]
	[JsonRequired]
	public required string RoomId { get; init; }

	/// <summary>
	/// Time when this event was sent
	/// </summary>
	[JsonProperty("timestamp")]
	[JsonRequired]
	public required ulong Timestamp { get; init; }

	/// <summary>
	/// Defines if the player has connected or disconnected
	/// </summary>
	[JsonProperty("event_type")]
	[JsonConverter(typeof(StringEqualConverter), "connected")]
	public required bool IsConnected { get; init; }
}
