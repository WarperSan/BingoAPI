using BingoAPI.Models;
using JetBrains.Annotations;

namespace BingoAPI.Goals;

/// <summary>
/// Interface that represents any class that can track the state of <see cref="Goal"/>
/// </summary>
[PublicAPI]
public interface IGoalTracker
{
	/// <summary>
	/// Begins the tracking for the given <see cref="Goal"/>
	/// </summary>
	public void Track(Goal goal);

	/// <summary>
	/// Ends the tracking for every tracked <see cref="Goal"/>
	/// </summary>
	public void UnTrackAll();

	/// <summary>
	/// Evaluates the given tracked <see cref="Goal"/>
	/// </summary>
	public void Evaluate(Goal goal);

	/// <summary>
	/// Evaluates all the tracked <see cref="Goal"/>
	/// </summary>
	public void EvaluateAll();

	/// <summary>
	/// Marks the given tracked <see cref="Goal"/>
	/// </summary>
	public void Mark(Goal goal);

	/// <summary>
	/// Unmarks the given tracked <see cref="Goal"/>
	/// </summary>
	public void UnMark(Goal goal);
}
