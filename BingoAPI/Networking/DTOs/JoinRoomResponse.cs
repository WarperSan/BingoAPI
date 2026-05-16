using Newtonsoft.Json;

namespace BingoAPI.Networking.DTOs;

/// <summary>
/// Model used as the response's payload of <see cref="BingoApiClient.JoinRoom"/>
/// </summary>
public record JoinRoomResponse
{
	[JsonProperty("socket_key")]
	[JsonRequired]
	public string SocketKey = string.Empty;
}
