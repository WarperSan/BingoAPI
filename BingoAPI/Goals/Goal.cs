using BingoAPI.Conditions;

namespace BingoAPI.Goals;

/// <summary>
/// Represents a bingo goal
/// </summary>
public sealed class Goal
{
	/// <summary>
	/// Name of this goal
	/// </summary>
	public string Name { get; }

	/// <summary>
	/// Condition that must be met for this goal to be completed
	/// </summary>
	public ICondition Condition { get; }

	internal Goal(string name, ICondition condition)
	{
		Name = name;
		Condition = condition;
	}
}
