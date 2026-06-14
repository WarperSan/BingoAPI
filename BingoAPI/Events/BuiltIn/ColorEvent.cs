using BingoAPI.Models;
using Newtonsoft.Json;

namespace BingoAPI.Events.BuiltIn;

/// <summary>
/// Event sent when a player changes team
/// </summary>
internal record ColorEvent : IEvent
{
	/// <summary>
	/// Player responsible for this event
	/// </summary>
	[JsonProperty("player")]
	[JsonRequired]
	public required Player Player { get; init; }

	/// <summary>
	/// Previous color of the player
	/// </summary>
	[JsonProperty("player_color")]
	[JsonRequired]
	public required Team PreviousColor { get; init; }

	/// <summary>
	/// New color of the player
	/// </summary>
	[JsonProperty("color")]
	[JsonRequired]
	public required Team NewColor { get; init; }

	/// <summary>
	/// Time when this event was sent
	/// </summary>
	[JsonProperty("timestamp")]
	[JsonRequired]
	public required ulong Timestamp { get; init; }
}
