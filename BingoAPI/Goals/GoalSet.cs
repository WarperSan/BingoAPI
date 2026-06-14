using Newtonsoft.Json;

namespace BingoAPI.Goals;

/// <summary>
/// Represents a set of <see cref="Goal"/>
/// </summary>
public sealed record GoalSet
{
	/// <summary>
	/// Name of this set
	/// </summary>
	[JsonProperty("name")]
	public string Name { get; init; } = string.Empty;

	/// <summary>
	/// Description of this goal
	/// </summary>
	[JsonProperty("description")]
	public string Description { get; init; } = string.Empty;

	/// <summary>
	/// List of the goals added by this set
	/// </summary>
	[JsonProperty("goals")]
	[JsonRequired]
	public required Goal[] Goals { get; init; }
}
