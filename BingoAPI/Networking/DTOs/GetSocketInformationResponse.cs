using Newtonsoft.Json;

namespace BingoAPI.Networking.DTOs;

/// <summary>
/// Model used as the response's payload of <see cref="BingoApiClient.GetSocketInformation"/>
/// </summary>
internal record GetSocketInformationResponse
{
	[JsonProperty("room")]
	[JsonRequired]
	public readonly string Code = string.Empty;

	[JsonProperty("player")]
	[JsonRequired]
	public readonly string PlayerUUID = string.Empty;
}
