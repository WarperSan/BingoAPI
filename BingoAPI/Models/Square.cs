using BingoAPI.Networking.Converters;
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
	public required string Name { get; init; }

	/// <summary>
	/// Index of this square
	/// </summary>
	[JsonProperty("slot")]
	[JsonRequired]
	public required SlotIndex Slot { get; init; }

	/// <summary>
	/// Teams currently owning this square
	/// </summary>
	[JsonProperty("colors")]
	[JsonRequired]
	public required Team Teams { get; init; }
}
