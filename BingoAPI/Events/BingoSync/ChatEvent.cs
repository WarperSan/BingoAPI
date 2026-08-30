using BingoAPI.Models;
using Newtonsoft.Json;

namespace BingoAPI.Events.BingoSync;

/// <summary>
/// Event sent when a player sends a message in the room
/// </summary>
internal sealed record ChatEvent : Event
{
	/// <summary>
	/// Player responsible for this event
	/// </summary>
	[JsonProperty("player")]
	[JsonRequired]
	public required Player Player { get; init; }

	/// <summary>
	/// Time when this event was sent
	/// </summary>
	[JsonProperty("timestamp")]
	[JsonRequired]
	public required ulong Timestamp { get; init; }

	/// <summary>
	/// Content of the message sent
	/// </summary>
	[JsonProperty("text")]
	[JsonRequired]
	public required string Text { get; init; }
}
