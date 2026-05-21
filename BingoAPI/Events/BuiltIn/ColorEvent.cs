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
	public readonly Player Player = null!;

	/// <summary>
	/// Previous color of the player
	/// </summary>
	[JsonProperty("player_color")]
	[JsonRequired]
	public readonly Team PreviousColor;

	/// <summary>
	/// New color of the player
	/// </summary>
	[JsonProperty("color")]
	[JsonRequired]
	public readonly Team NewColor;

	/// <summary>
	/// Time when this event was sent
	/// </summary>
	[JsonProperty("timestamp")]
	[JsonRequired]
	public readonly ulong Timestamp;
}
