using JetBrains.Annotations;

namespace BingoAPI.Configuration.Settings;

/// <summary>
/// Settings used when joining a room
/// </summary>
[PublicAPI]
public record JoinRoomSettings
{
	/// <summary>
	/// Code of the room
	/// </summary>
	public required string Code { get; init; }

	/// <summary>
	/// Password of the room
	/// </summary>
	public required string Password { get; init; }

	/// <summary>
	/// Name of the player to connect as
	/// </summary>
	public required string Nickname { get; init; }

	/// <summary>
	/// Defines if the player will join as a spectator
	/// </summary>
	public bool IsSpectator { get; init; }
}
