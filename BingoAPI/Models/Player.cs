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
	public string UUID = string.Empty;

	/// <summary>
	/// Display name of this player
	/// </summary>
	[JsonProperty("name")]
	[JsonRequired]
	public string Name = string.Empty;

	/// <summary>
	/// Team this player belongs to
	/// </summary>
	[JsonProperty("color")]
	[JsonRequired]
	public Team Team = Team.None;
}
