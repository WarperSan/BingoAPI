using BingoAPI.Networking;
using JetBrains.Annotations;

namespace BingoAPI.Models.Settings;

/// <summary>
/// Data used to join a room when calling <see cref="Session.CreateRoom"/>
/// </summary>
[PublicAPI]
public record CreateRoomSettings
{
	/// <summary>
	/// Name of the room
	/// </summary>
	public string Name { get; set; } = string.Empty;

	/// <summary>
	/// Password of the room
	/// </summary>
	public string Password { get; set; } = string.Empty;

	/// <inheritdoc cref="JoinRoomSettings.Nickname"/>
	public string Nickname { get; set; } = string.Empty;

	/// <summary>
	/// Should the room be randomized or not
	/// </summary>
	public bool IsRandomized { get; set; }

	/// <summary>
	/// Should the room be in lockout or not
	/// </summary>
	public bool IsLockout { get; set; }

	/// <summary>
	/// Seed to use for the randomization
	/// </summary>
	/// <remarks>
	/// Leave it empty if you want the seed to be automatically generated
	/// </remarks>
	public string Seed { get; set; } = string.Empty;
}
