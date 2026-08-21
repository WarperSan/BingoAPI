using System.Collections;
using System.Diagnostics.CodeAnalysis;
using BingoAPI.Models;
using JetBrains.Annotations;

namespace BingoAPI.Goals;

/// <summary>
/// Collection of <see cref="Goal"/> instances, accessible by name
/// </summary>
[PublicAPI]
public sealed class GoalPool : IGoalPool
{
	// TODO: Implement collision-safe ID

	private readonly Dictionary<string, Goal> _goals = new(StringComparer.OrdinalIgnoreCase);

	/// <inheritdoc />
	public int Count => _goals.Count;

	/// <inheritdoc />
	public bool IsReadOnly => false;

	/// <inheritdoc />
	public bool TryGetValue(Square square, [NotNullWhen(true)] out Goal? goal)
	{
		return _goals.TryGetValue(square.Text, out goal);
	}

	/// <inheritdoc />
	public bool TryAdd(Goal goal)
	{
		if (IsReadOnly)
			throw new InvalidOperationException("The pool is read-only.");

		if (Contains(goal))
			return false;

		_goals.Add(goal.Name, goal);
		return true;
	}

	/// <inheritdoc />
	public void Add(Goal item)
	{
		if (TryAdd(item))
			return;

		throw new ArgumentException("The goal has already been added.", nameof(item));
	}

	/// <inheritdoc />
	public void Clear()
	{
		if (IsReadOnly)
			throw new InvalidOperationException("The pool is read-only.");

		_goals.Clear();
	}

	/// <inheritdoc />
	public bool Contains(Goal item)
	{
		return _goals.ContainsKey(item.Name);
	}

	/// <inheritdoc />
	public void CopyTo(Goal[] array, int arrayIndex)
	{
		if (array == null)
			throw new ArgumentNullException(nameof(array));

		if (arrayIndex < 0)
			throw new ArgumentOutOfRangeException(nameof(arrayIndex));

		if (array.Length - arrayIndex < Count)
			throw new ArgumentException(nameof(arrayIndex));

		var i = arrayIndex;

		foreach (var goal in this)
		{
			array[i] = goal;
			i++;
		}
	}

	/// <inheritdoc />
	public bool Remove(Goal item)
	{
		if (IsReadOnly)
			throw new InvalidOperationException("The pool is read-only.");

		if (!Contains(item))
			return false;

		return _goals.Remove(item.Name);
	}

	/// <inheritdoc />
	public IEnumerator<Goal> GetEnumerator() => _goals.Values.GetEnumerator();

	/// <inheritdoc />
	IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
}
