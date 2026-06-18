using Newtonsoft.Json;

namespace BingoAPI.Models;

/// <summary>
/// Represents a player in the room
/// </summary>
public record Player
{
	/// <summary>
	/// Unique identifier of this player
	/// </summary>
	[JsonProperty("uuid")]
	[JsonRequired]
	public required string UUID { get; init; }

	/// <summary>
	/// Display name of this player
	/// </summary>
	[JsonProperty("name")]
	[JsonRequired]
	public required string Name { get; init; }

	/// <summary>
	/// Team this player belongs to
	/// </summary>
	[JsonProperty("color")]
	[JsonRequired]
	public required Team Team { get; init; }
}
