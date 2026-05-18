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
	public readonly string UUID = string.Empty;

	/// <summary>
	/// Display name of this player
	/// </summary>
	[JsonProperty("name")]
	[JsonRequired]
	public readonly string Name = string.Empty;

	/// <summary>
	/// Team this player belongs to
	/// </summary>
	[JsonProperty("color")]
	[JsonRequired]
	public readonly Team Team = Team.None;
}
