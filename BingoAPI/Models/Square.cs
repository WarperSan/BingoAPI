namespace BingoAPI.Models;

/// <summary>
/// Represents a square on a bingo card
/// </summary>
public record Square
{
	/// <summary>
	/// Text displayed on this square
	/// </summary>
	public required string Text { get; init; }

	/// <summary>
	/// Index of this square
	/// </summary>
	public required int Index { get; init; }

	/// <summary>
	/// Teams currently owning this square
	/// </summary>
	public required Team Teams { get; init; }
}
