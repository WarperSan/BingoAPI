using BingoAPI.Clients.BingoSync;
using BingoAPI.Configuration.Settings;
using Newtonsoft.Json;

namespace BingoAPI.DTOs.BingoSync.JoinRoom;

/// <summary>
/// Model used as the response of <see cref="BingoSyncApiClient.JoinRoom(JoinRoomSettings,CancellationToken)"/>
/// </summary>
internal record Response
{
	[JsonProperty("socket_key")]
	[JsonRequired]
	public required string SocketKey { get; init; }
}
