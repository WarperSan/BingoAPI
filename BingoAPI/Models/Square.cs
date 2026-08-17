using JetBrains.Annotations;

namespace BingoAPI.Models;

/// <summary>
/// Represents a square on a bingo card
/// </summary>
[PublicAPI]
public record Square
{
	/// <summary>
	/// Text displayed on this square
	/// </summary>
	public required string Text { get; init; }

	/// <summary>
	/// Index of this square
	/// </summary>
	/// <remarks>
	/// This index is 0-based
	/// </remarks>
	public required int Index { get; init; }

	/// <summary>
	/// Teams currently owning this square
	/// </summary>
	public required Team Teams { get; init; }
}
