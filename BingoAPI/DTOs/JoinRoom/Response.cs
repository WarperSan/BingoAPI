using Newtonsoft.Json;

namespace BingoAPI.DTOs.JoinRoom;

internal record Response
{
	[JsonProperty("socket_key")]
	[JsonRequired]
	public required string SocketKey { get; init; }
}
