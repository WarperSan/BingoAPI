using BingoAPI.Networking.DTOs;
using Newtonsoft.Json;

namespace BingoAPI.Models;

/// <summary>
/// Represents a square on a bingo card
/// </summary>
public record Square
{
	/// <summary>
	/// Text displayed on this square
	/// </summary>
	[JsonProperty("name")]
	[JsonRequired]
	public required string Text { get; init; }

	/// <summary>
	/// Index of this square
	/// </summary>
	[JsonProperty("slot")]
	[JsonRequired]
	private SlotIndex? _slot;

	/// <summary>
	/// Index of this square
	/// </summary>
	public int Index => _slot?.Index ?? 0;

	/// <summary>
	/// Teams currently owning this square
	/// </summary>
	[JsonProperty("colors")]
	[JsonRequired]
	public required Team Teams { get; init; }
}
