using BingoAPI.Conditions;
using JetBrains.Annotations;

namespace BingoAPI.Models;

/// <summary>
/// Represents a bingo goal
/// </summary>
[PublicAPI]
public record Goal
{
	/// <summary>
	/// Name of this goal
	/// </summary>
	public required string Name { get; init; }

	/// <summary>
	/// Condition that must be met for this goal to be completed
	/// </summary>
	public required ICondition Condition { get; init; }
}
