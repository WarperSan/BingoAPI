using BingoAPI.Networking.Converters;
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
	public readonly string Name = string.Empty;

	/// <summary>
	/// Index of this square
	/// </summary>
	[JsonProperty("slot")]
	[JsonRequired]
	[JsonConverter(typeof(SlotIndexConverter))]
	public readonly int Index;

	/// <summary>
	/// Teams currently owning this square
	/// </summary>
	[JsonProperty("colors")]
	[JsonRequired]
	public readonly Team Teams;
}
