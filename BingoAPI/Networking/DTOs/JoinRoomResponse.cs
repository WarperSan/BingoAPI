using BingoAPI.Networking.Clients;
using Newtonsoft.Json;

namespace BingoAPI.Networking.DTOs;

/// <summary>
/// Model used as the response's payload of <see cref="BingoApiClient.JoinRoom"/>
/// </summary>
internal record JoinRoomResponse
{
	[JsonProperty("socket_key")]
	[JsonRequired]
	public required string SocketKey { get; init; }
}
