using BingoAPI.Conditions;
using BingoAPI.Logging;
using BingoAPI.Models;
using JetBrains.Annotations;

namespace BingoAPI.Goals.BuiltIn;

/// <summary>
/// Tracks a fixed set of <see cref="Goal"/> and notifies when their <see cref="ICondition"/> is met or cleared
/// </summary>
[PublicAPI]
public sealed class GoalTracker : IGoalTracker
{
	private readonly ILogger _logger;
	private readonly HashSet<Goal> _trackedGoals = [];
	private readonly HashSet<Goal> _metGoals = [];

	/// <summary>
	/// Initializes a new instance of the <see cref="GoalTracker"/> class.
	/// </summary>
	public GoalTracker(ILogger logger)
	{
		_logger = logger;
	}

	/// <inheritdoc />
	public void Track(Goal goal)
	{
		_trackedGoals.Add(goal);
	}

	/// <inheritdoc />
	public void UnTrackAll()
	{
		_trackedGoals.Clear();
		_metGoals.Clear();
	}

	/// <inheritdoc />
	public void Evaluate(Goal goal)
	{
		if (!_trackedGoals.Contains(goal))
			throw new ArgumentException($"Cannot evaluate a '{nameof(Goal)}' not tracked.");

		var wasMet = _metGoals.Contains(goal);

		bool isMet;

		try
		{
			isMet = goal.Condition.IsMet();
		}
		catch (Exception e)
		{
			_logger.Error($"Error while evaluating '{goal.Name}': {e}");
			isMet = false;
		}

		// If state didn't change, skip
		if (wasMet == isMet)
			return;

		if (isMet)
			Mark(goal);
		else
			UnMark(goal);
	}

	/// <inheritdoc />
	public void EvaluateAll()
	{
		foreach (var goal in _trackedGoals)
			Evaluate(goal);
	}

	/// <inheritdoc />
	public void Mark(Goal goal)
	{
		if (!_trackedGoals.Contains(goal))
			throw new ArgumentException($"Cannot evaluate a '{nameof(Goal)}' not tracked.");

		_metGoals.Add(goal);
		OnGoalMarked?.Invoke(goal);
	}

	/// <inheritdoc />
	public void UnMark(Goal goal)
	{
		if (!_trackedGoals.Contains(goal))
			throw new ArgumentException($"Cannot evaluate a '{nameof(Goal)}' not tracked.");

		_metGoals.Remove(goal);
		OnGoalCleared?.Invoke(goal);
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

	#endregion
}
