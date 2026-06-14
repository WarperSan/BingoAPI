using BingoAPI.Networking.Clients;
using Newtonsoft.Json;

namespace BingoAPI.Networking.DTOs;

/// <summary>
/// Model used as the response's payload of <see cref="BingoApiClient.GetSocketInformation"/>
/// </summary>
internal record GetSocketInformationResponse
{
	[JsonProperty("room")]
	[JsonRequired]
	public required string Code { get; init; }

	[JsonProperty("player")]
	[JsonRequired]
	public required string PlayerUUID { get; init; }
}
