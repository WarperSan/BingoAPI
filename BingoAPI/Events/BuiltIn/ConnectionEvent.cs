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
	public readonly Player Player = null!;

	/// <summary>
	/// Identifier of the room
	/// </summary>
	[JsonProperty("room")]
	[JsonRequired]
	public readonly string RoomId = string.Empty;

	/// <summary>
	/// Time when this event was sent
	/// </summary>
	[JsonProperty("timestamp")]
	[JsonRequired]
	public readonly ulong Timestamp;

	/// <summary>
	/// Defines if the player has connected or disconnected
	/// </summary>
	[JsonProperty("event_type")]
	[JsonConverter(typeof(StringEqualConverter), "connected")]
	public readonly bool IsConnected;
}
