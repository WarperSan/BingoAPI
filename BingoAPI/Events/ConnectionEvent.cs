using BingoAPI.Models;
using BingoAPI.Networking.Converters;
using Newtonsoft.Json;

namespace BingoAPI.Events;

/// <summary>
/// Event sent when a player joins or leaves the room
/// </summary>
[JsonConverter(typeof(EventConverter))]
public record ConnectionEvent : IBingoEvent
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

	/// <summary>
	/// Defines if the player has connected or disconnected
	/// </summary>
	[JsonIgnore]
	public bool IsConnected { get; internal set; }
}
