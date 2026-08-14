using Newtonsoft.Json;

namespace BingoAPI.DTOs.RevealCard;

internal record Request
{
	[JsonProperty("room")]
	public required string Code { get; init; }
}
