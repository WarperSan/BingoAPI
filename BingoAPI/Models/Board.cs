namespace BingoAPI.Models;

public sealed class Board
{
	public IReadOnlyList<Square> Squares { get; }

	public Board(IReadOnlyList<Square> squares)
	{
		Squares = squares;
	}
}
