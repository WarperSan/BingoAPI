using BingoAPI.Networking.Converters;
using Newtonsoft.Json;

namespace BingoAPI.Models;

public record Square
{
	/// <summary>
	/// Text displayed on this square
	/// </summary>
	[JsonProperty("name")]
	[JsonRequired]
	public string Name = string.Empty;

	/// <summary>
	/// Index of this square
	/// </summary>
	[JsonProperty("slot")]
	[JsonRequired]
	[JsonConverter(typeof(SlotIndexConverter))]
	public int Index;

	/// <summary>
	/// Teams currently owning this square
	/// </summary>
	[JsonProperty("colors")]
	[JsonRequired]
	public Team Teams = Team.None;
}
