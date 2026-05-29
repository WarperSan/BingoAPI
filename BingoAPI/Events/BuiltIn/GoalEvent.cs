using BingoAPI.Models;
using Newtonsoft.Json;

namespace BingoAPI.Events.BuiltIn;

/// <summary>
/// Event sent when a player marks or clears a square
/// </summary>
internal record GoalEvent : IEvent
{
	/// <summary>
	/// Player responsible for this event
	/// </summary>
	[JsonProperty("player")]
	[JsonRequired]
	public readonly Player Player = null!;

	/// <summary>
	/// Time when this event was sent
	/// </summary>
	[JsonProperty("timestamp")]
	[JsonRequired]
	public readonly ulong Timestamp;

	/// <summary>
	/// Square modified by this event
	/// </summary>
	[JsonProperty("square")]
	[JsonRequired]
	public readonly Square Square = null!;

	/// <summary>
	/// Team that was added or removed
	/// </summary>
	[JsonProperty("color")]
	[JsonRequired]
	public readonly Team Team;

	/// <summary>
	/// Defines if the selected square has been cleared or marked
	/// </summary>
	[JsonProperty("remove")]
	[JsonRequired]
	public readonly bool HasBeenCleared;
}
