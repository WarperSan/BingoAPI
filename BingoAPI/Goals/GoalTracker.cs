using BingoAPI.Conditions;
using BingoAPI.Helpers;
using JetBrains.Annotations;

namespace BingoAPI.Goals;

/// <summary>
/// Tracks a fixed set of <see cref="Goal"/> and notifies when their <see cref="ICondition"/> is met or cleared
/// </summary>
[PublicAPI]
public sealed class GoalTracker
{
	private readonly HashSet<Goal> _trackedGoals = [];
	private readonly HashSet<Goal> _metGoals = [];

	/// <summary>
	/// Tries to add the given <see cref="Goal"/> to the tracked list
	/// </summary>
	/// <returns>Success of the attempt</returns>
	public bool TryAdd(Goal goal) => _trackedGoals.Add(goal);

	/// <summary>
	/// Clears every tracked <see cref="Goal"/>
	/// </summary>
	public void Clear()
	{
		_trackedGoals.Clear();
		_metGoals.Clear();
	}

	/// <summary>
	/// Evaluates every tracked <see cref="Goal"/> to see if any update happened
	/// </summary>
	public void Evaluate()
	{
		foreach (var goal in _trackedGoals)
		{
			var wasMet = _metGoals.Contains(goal);

			bool isMet;

			try
			{
				isMet = goal.Condition.IsMet();
			}
			catch (Exception e)
			{
				Log.Error($"Error while evaluating '{goal.Name}': {e}");
				isMet = false;
			}

			DispatchChange(wasMet, isMet, goal);
		}
	}

	#region Callbacks

	/// <summary>
	/// Callback used when a goal changes status
	/// </summary>
	public delegate void GoalChangedCallback(Goal goal);

	/// <summary>
	/// Called when a <see cref="Goal"/> was newly marked
	/// </summary>
	public event GoalChangedCallback? OnGoalMarked;

	/// <summary>
	/// Called when a <see cref="Goal"/> was newly cleared
	/// </summary>
	public event GoalChangedCallback? OnGoalCleared;

	/// <summary>
	/// Dispatches the change of the given <see cref="Goal"/>
	/// </summary>
	private void DispatchChange(bool wasInitiallyMet, bool isCurrentlyMet, Goal goal)
	{
		if (wasInitiallyMet == isCurrentlyMet)
			return;

		if (isCurrentlyMet)
		{
			_metGoals.Add(goal);
			OnGoalMarked?.Invoke(goal);
		}
		else
		{
			_metGoals.Remove(goal);
			OnGoalCleared?.Invoke(goal);
		}
	}

	#endregion
}
