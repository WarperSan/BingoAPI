using BingoAPI.Clients.BingoSync;
using BingoAPI.Configuration.Settings;
using Newtonsoft.Json;

namespace BingoAPI.DTOs.BingoSync.JoinRoom;

/// <summary>
/// Model used as the payload of <see cref="BingoSyncApiClient.JoinRoom(JoinRoomSettings,CancellationToken)"/>
/// </summary>
internal class Request
{
	[JsonProperty("room")]
	[JsonRequired]
	public required string Code { get; init; }

	[JsonProperty("password")]
	[JsonRequired]
	public required string Password { get; init; }

	[JsonProperty("nickname")]
	[JsonRequired]
	public required string Username { get; init; }

	[JsonProperty("is_spectator")]
	[JsonRequired]
	public required bool IsSpectator { get; init; }
}
