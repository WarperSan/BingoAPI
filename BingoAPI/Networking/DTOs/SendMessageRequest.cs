using BingoAPI.Networking.Clients;
using Newtonsoft.Json;

namespace BingoAPI.Networking.DTOs;

/// <summary>
/// Model used as the request's payload of <see cref="BingoApiClient.SendMessage"/>
/// </summary>
internal record SendMessageRequest
{
	[JsonProperty("room")]
	public required string Code { get; init; }

	[JsonProperty("text")]
	public required string Message { get; init; }
}
