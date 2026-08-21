using JetBrains.Annotations;

namespace BingoAPI.Models;

/// <summary>
/// Represents a player in the room
/// </summary>
[PublicAPI]
public sealed record Player
{
	/// <summary>
	/// Unique identifier of this player
	/// </summary>
	public required string UUID { get; init; }

	/// <summary>
	/// Display name of this player
	/// </summary>
	public required string Name { get; init; }

	/// <summary>
	/// Team this player belongs to
	/// </summary>
	public required Team Team { get; init; }
}
