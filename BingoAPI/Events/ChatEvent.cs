using BingoAPI.Models;
using Newtonsoft.Json;

namespace BingoAPI.Events;

/// <summary>
/// Event sent when a player sends a message in the room
/// </summary>
public record ChatEvent : IBingoEvent
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
	/// Content of the message sent
	/// </summary>
	[JsonProperty("text")]
	[JsonRequired]
	public string Text = string.Empty;
}
