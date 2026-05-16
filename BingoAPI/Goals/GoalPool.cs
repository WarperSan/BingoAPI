using BingoAPI.Helpers;

namespace BingoAPI.Goals;

/// <summary>
/// Holds all <see cref="Goal"/> instances available for a bingo match
/// </summary>
public sealed class GoalPool
{
	private readonly Dictionary<string, Goal> _goals = new(StringComparer.OrdinalIgnoreCase);

	/// <summary>
	/// All <see cref="Goal"/> currently in the pool
	/// </summary>
	public IReadOnlyCollection<Goal> Goals => _goals.Values;

	/// <summary>
	/// Adds the given <see cref="Goal"/> to the pool
	/// </summary>
	/// <remarks>
	/// If two instances in the pool have the same <see cref="Goal.Name"/>, only the first one will be kept
	/// </remarks>
	public void Add(IEnumerable<Goal> goals)
	{
		foreach (var goal in goals)
		{
			if (_goals.ContainsKey(goal.Name))
			{
				Log.Warning($"Goal skipped, because another goal has the same name: '{goal.Name}'");
				continue;
			}
			_goals.Add(goal.Name, goal);
		}
	}

	/// <summary>
	/// Finds a <see cref="Goal"/> by name
	/// </summary>
	public Goal? Find(string name)
	{
		if (_goals.TryGetValue(name, out var goal))
			return goal;

		return null;
	}
}
