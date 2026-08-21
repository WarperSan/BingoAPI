using BingoAPI.Goals;
using BingoAPI.Logging;
using JetBrains.Annotations;

namespace BingoAPI.Models;

/// <summary>
/// Represents the current state of a bingo card
/// </summary>
[PublicAPI]
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
	private readonly ILogger _logger;

	/// <summary>
	/// Size of the card on both axes
	/// </summary>
	public readonly int Size;

	/// <summary>
	/// Initializes a new instance of the <see cref="Card"/> class.
	/// </summary>
	public Card(ICollection<Square> squares, IGoalPool pool, ILogger logger)
	{
		if (squares.Count == 0)
			throw new ArgumentException(
				"Tried to create a card without providing any square.",
				nameof(squares)
			);

		var size = (int)Math.Sqrt(squares.Count);

		if (size * size != squares.Count)
			throw new ArgumentException(
				$"Card must be a perfect square, but received '{size}'.",
				nameof(squares)
			);

		_squares = new CardSquare[squares.Count];
		_logger = logger;
		Size = size;

		foreach (var square in squares)
		{
			var index = square.Index;

			if (index < 0 || index >= _squares.Length)
				throw new ArgumentOutOfRangeException(nameof(square));

			// TODO: Consider if this is the responsibility of the card or of the pool
			if (!pool.TryGetValue(square, out var goal))
			{
				logger.Error(
					$"Failed to find a goal under the name '{square.Text}', defaulting to manual."
				);

				goal = new Goal
				{
					Name = square.Text,
					Condition = null,
					// TODO: Condition = new ManualCondition()
				};
			}

			_squares[index] = new CardSquare(square, goal);
		}
	}

	/// <summary>
	/// Gets the <see cref="Goal"/> at the given index
	/// </summary>
	public Goal GetGoalAt(int index) => _squares[index].Goal;

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
}
