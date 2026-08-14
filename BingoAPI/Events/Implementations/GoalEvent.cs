using BingoAPI.Events.Interfaces;
using BingoAPI.Models;
using Newtonsoft.Json;

namespace BingoAPI.Events.Implementations;

/// <summary>
/// Event sent when a player marks or clears a square
/// </summary>
internal record GoalEvent : IGoalEvent
{
	/// <inheritdoc/>
	[JsonProperty("player")]
	[JsonRequired]
	public required Player Player { get; init; }

	/// <inheritdoc/>
	[JsonProperty("square")]
	[JsonRequired]
	public required Square Square { get; init; }

	/// <inheritdoc/>
	[JsonProperty("color")]
	[JsonRequired]
	public required Team Team { get; init; }

	/// <inheritdoc/>
	[JsonProperty("remove")]
	[JsonRequired]
	public required bool HasBeenCleared { get; init; }
}
