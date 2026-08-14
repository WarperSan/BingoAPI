using Newtonsoft.Json;

namespace BingoAPI.DTOs.GetSocketInformation;

internal record Response
{
	[JsonProperty("room")]
	[JsonRequired]
	public required string Code { get; init; }

	[JsonProperty("player")]
	[JsonRequired]
	public required string PlayerUUID { get; init; }
}
