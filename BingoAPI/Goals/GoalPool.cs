using System.Diagnostics.CodeAnalysis;

namespace BingoAPI.Goals;

/// <summary>
/// Collection of <see cref="Goal"/> instances, accessible by name
/// </summary>
[SuppressMessage("ReSharper", "ClassNeverInstantiated.Global")]
[SuppressMessage("ReSharper", "UnusedMember.Global")]
public sealed class GoalPool
{
	private readonly Dictionary<string, Goal> _goals = new(StringComparer.OrdinalIgnoreCase);

	/// <summary>
	/// Number of <see cref="Goal"/> stored in this pool
	/// </summary>
	public int Count => _goals.Count;

	/// <summary>
	/// Adds the given <see cref="Goal"/> to this pool
	/// </summary>
	public void Add(Goal goal)
	{
		var succeeded = TryAdd(goal);

		if (!succeeded)
			throw new ArgumentException("The goal has already been added.", nameof(goal));
	}

	/// <summary>
	/// Tries to add the given <see cref="Goal"/> to this pool
	/// </summary>
	/// <returns>Success of the attempt</returns>
	public bool TryAdd(Goal goal)
	{
		if (_goals.ContainsKey(goal.Name))
			return false;

		_goals.Add(goal.Name, goal);
		return true;
	}

	/// <summary>
	/// Tries to get the <see cref="Goal"/> from the given name
	/// </summary>
	/// <param name="name">Name of the target goal</param>
	/// <param name="goal">Matched goal, or <c>null</c> if not found</param>
	/// <returns>Success of the attempt</returns>
	public bool TryGet(string name, out Goal? goal) => _goals.TryGetValue(name, out goal);
}
