using BingoAPI.Models;
using Newtonsoft.Json;

namespace BingoAPI.Events;

/// <summary>
/// Event sent when a player joins or leaves the room
/// </summary>
internal record ConnectionEvent : IBingoEvent
{
	/// <summary>
	/// Player responsible for this event
	/// </summary>
	[JsonProperty("player")]
	[JsonRequired]
	public Player Player = null!;

	/// <summary>
	/// Identifier of the room
	/// </summary>
	[JsonProperty("room")]
	[JsonRequired]
	public string RoomId = string.Empty;

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
