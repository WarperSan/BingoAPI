using BingoAPI.Models;
using Newtonsoft.Json;

namespace BingoAPI.Events;

/// <summary>
/// Event sent when a player changes team
/// </summary>
internal record ColorEvent : IBingoEvent
{
	/// <summary>
	/// Player responsible for this event
	/// </summary>
	[JsonProperty("player")]
	[JsonRequired]
	public Player Player = null!;

	/// <summary>
	/// Previous color of the player
	/// </summary>
	[JsonProperty("player_color")]
	[JsonRequired]
	public Team PreviousColor;

	/// <summary>
	/// New color of the player
	/// </summary>
	[JsonProperty("color")]
	[JsonRequired]
	public Team NewColor;

	/// <summary>
	/// Time when this event was sent
	/// </summary>
	[JsonProperty("timestamp")]
	[JsonRequired]
	public ulong Timestamp;
}
