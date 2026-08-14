using Newtonsoft.Json;

namespace BingoAPI.DTOs.SendMessage;

internal record Request
{
	[JsonProperty("room")]
	public required string Code { get; init; }

	[JsonProperty("text")]
	public required string Message { get; init; }
}
