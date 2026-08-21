using JetBrains.Annotations;

namespace BingoAPI.Configuration.Settings;

/// <summary>
/// Settings used when creating a room
/// </summary>
[PublicAPI]
public sealed record CreateRoomSettings
{
	/// <summary>
	/// Name of the room
	/// </summary>
	public required string Name { get; init; }

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

	/// <summary>
	/// Should the room be randomized or not
	/// </summary>
	public bool IsRandomized { get; set; }

	/// <summary>
	/// Should the room be in lockout or not
	/// </summary>
	public bool IsLockout { get; set; }

	/// <summary>
	/// Defines if the card should be hidden initially
	/// </summary>
	public bool HideCard { get; init; }

	/// <summary>
	/// Seed to use for the randomization
	/// </summary>
	/// <remarks>
	/// Leave it empty if you want the seed to be automatically generated
	/// </remarks>
	public string Seed { get; set; } = string.Empty;
}
