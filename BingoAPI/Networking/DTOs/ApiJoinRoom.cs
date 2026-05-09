using Newtonsoft.Json;

namespace BingoAPI.Networking.DTOs;

/// <summary>
/// Model used as the request's payload of <see cref="BingoSyncApi.JoinRoom"/>
/// </summary>
public record ApiJoinRoomRequest
{
	[JsonProperty("room")]
	public string Code = string.Empty;

	[JsonProperty("password")]
	public string Password = string.Empty;

	[JsonProperty("nickname")]
	public string Username = string.Empty;

	[JsonProperty("is_spectator")]
	public bool IsSpectator;
}

/// <summary>
/// Model used as the response's payload of <see cref="BingoSyncApi.JoinRoom"/>
/// </summary>
public record ApiJoinRoomResponse
{
	[JsonProperty("socket_key")]
	[JsonRequired]
	public string SocketKey = string.Empty;
}
