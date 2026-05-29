using System.Diagnostics.CodeAnalysis;
using BingoAPI.Networking.Clients;

namespace BingoAPI.Models.Settings;

/// <summary>
/// Data used to join a room when calling <see cref="BingoApiClient.JoinRoom"/>
/// </summary>
[SuppressMessage("ReSharper", "AutoPropertyCanBeMadeGetOnly.Global")]
[SuppressMessage("ReSharper", "ClassNeverInstantiated.Global")]
public record JoinRoomSettings
{
	/// <summary>
	/// Code of the room
	/// </summary>
	public string Code { get; set; } = string.Empty;

	/// <summary>
	/// Password of the room
	/// </summary>
	public string Password { get; set; } = string.Empty;

	/// <summary>
	/// Name of the player to connect as
	/// </summary>
	public string Nickname { get; set; } = string.Empty;

	/// <summary>
	/// Should the user be connected as a spectator or not
	/// </summary>
	public bool IsSpectator { get; set; }
}
