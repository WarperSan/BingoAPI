using BingoAPI.Goals;

namespace BingoAPI.Models;

/// <summary>
/// Represents the current state of a bingo card
/// </summary>
public sealed class Card
{
	private struct CardSquare
	{
		public CardSquare(Square square, Goal goal)
		{
			Goal = goal;
			Teams = square.Teams;
		}

		/// <summary>
		/// <see cref="Goal"/> this square is associated with
		/// </summary>
		public readonly Goal Goal;

		/// <summary>
		/// Teams who marked this square
		/// </summary>
		public Team Teams { get; set; }
	}

	private readonly CardSquare[] _squares;

	/// <summary>
	/// Size of the card on both axes
	/// </summary>
	public int Size { get; init; }

	internal Card(Square[] squares, GoalPool pool)
	{
		if (squares.Length == 0)
			throw new ArgumentException("Tried to create a card without providing any square.");

		var size = (int)Math.Sqrt(squares.Length);

		if (size * size != squares.Length)
			throw new ArgumentException($"Card must be a perfect square, but received '{size}'.");

		Size = size;

		_squares = new CardSquare[squares.Length];

		foreach (var square in squares)
		{
			var index = square.Slot.Index;

			if (index < 0 || index >= _squares.Length)
				throw new ArgumentOutOfRangeException(nameof(square));

			if (!pool.TryGet(square, out var goal))
				throw new KeyNotFoundException($"Failed to find a goal under the name '{square.Text}'.");

			_squares[index] = new CardSquare(square, goal);
		}
	}

	#region Goals

	/// <summary>
	/// Gets the <see cref="Goal"/> at the given index
	/// </summary>
	public Goal GetGoalAt(int index) => _squares[index].Goal;

	/// <summary>
	/// Gets all <see cref="Goal"/> instances present in this collection
	/// </summary>
	public Goal[] GetAllGoals()
	{
		var goals = new HashSet<Goal>();

		foreach (var square in _squares)
			goals.Add(square.Goal);

		return goals.ToArray();
	}

	/// <summary>
	/// Finds all indices where the given <see cref="Goal"/> appears
	/// </summary>
	public int[] FindByGoal(Goal goal)
	{
		var indices = new List<int>();

		for (var i = 0; i < _squares.Length; i++)
		{
			if (GetGoalAt(i) != goal)
				continue;

			indices.Add(i);
		}

		return indices.ToArray();
	}

	#endregion

	#region Teams

	/// <summary>
	/// Gets all <see cref="Team"/> that marked the square at the given index
	/// </summary>
	public Team GetTeamsAt(int index) => _squares[index].Teams;

	/// <summary>
	/// Checks if the square at the given index is marked by the given <see cref="Team"/>
	/// </summary>
	public bool IsMarkedBy(int index, Team team) => GetTeamsAt(index).HasFlag(team);

	/// <summary>
	/// Marks the square at the given index for the given <see cref="Team"/>
	/// </summary>
	public void Mark(int index, Team team) => _squares[index].Teams |= team;

	/// <summary>
	/// Clears the square at the given index for the given <see cref="Team"/>
	/// </summary>
	public void Unmark(int index, Team team) => _squares[index].Teams &= ~team;

	#endregion
}
