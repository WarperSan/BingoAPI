using BingoAPI.Conditions;
using Newtonsoft.Json;

namespace BingoAPI.Goals;

/// <summary>
/// Represents a bingo goal
/// </summary>
public sealed record Goal
{
	/// <summary>
	/// Name of this goal
	/// </summary>
	[JsonProperty("name")]
	[JsonRequired]
	public required string Name { get; init; }

	/// <summary>
	/// Condition that must be met for this goal to be completed
	/// </summary>
	[JsonProperty("condition")]
	[JsonRequired]
	public required ICondition Condition { get; init; }
}
