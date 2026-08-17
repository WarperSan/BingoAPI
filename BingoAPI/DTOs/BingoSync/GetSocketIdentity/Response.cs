using Newtonsoft.Json;

namespace BingoAPI.DTOs.BingoSync.GetSocketIdentity;

internal class Response
{
	[JsonProperty("room")]
	[JsonRequired]
	public required string Code { get; init; }

	[JsonProperty("player")]
	[JsonRequired]
	public required string PlayerUUID { get; init; }
}
