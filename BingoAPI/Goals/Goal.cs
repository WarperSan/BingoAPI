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
	public readonly string Name = string.Empty;

	/// <summary>
	/// Condition that must be met for this goal to be completed
	/// </summary>
	[JsonProperty("condition")]
	[JsonRequired]
	public readonly ICondition Condition = null!;
}
