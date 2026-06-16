using System.Diagnostics.CodeAnalysis;
using BingoAPI.Models;
using JetBrains.Annotations;

namespace BingoAPI.Goals;

/// <summary>
/// Collection of <see cref="Goal"/> instances, accessible by name
/// </summary>
[PublicAPI]
public sealed class GoalPool
{
	// TODO: Implement collision-safe ID

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
	/// Attempts to get the <see cref="Goal"/> represented by the given <see cref="Square"/>
	/// </summary>
	public bool TryGet(Square square, [NotNullWhen(true)] out Goal? goal) => _goals.TryGetValue(square.Text, out goal);
}
