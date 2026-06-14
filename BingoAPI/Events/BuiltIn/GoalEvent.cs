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
	public required Player Player { get; init; }

	/// <summary>
	/// Time when this event was sent
	/// </summary>
	[JsonProperty("timestamp")]
	[JsonRequired]
	public required ulong Timestamp { get; init; }

	/// <summary>
	/// Square modified by this event
	/// </summary>
	[JsonProperty("square")]
	[JsonRequired]
	public required Square Square { get; init; }

	/// <summary>
	/// Team that was added or removed
	/// </summary>
	[JsonProperty("color")]
	[JsonRequired]
	public required Team Team { get; init; }

	/// <summary>
	/// Defines if the selected square has been cleared or marked
	/// </summary>
	[JsonProperty("remove")]
	[JsonRequired]
	public required bool HasBeenCleared { get; init; }
}
