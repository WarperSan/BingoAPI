using Newtonsoft.Json;

namespace BingoAPI.Networking.DTOs;

/// <summary>
/// Model used as the request's payload of <see cref="BingoApiClient.MarkSquare"/>
/// </summary>
public record ApiRevealCardRequest
{
	[JsonProperty("room")]
	public string Code = string.Empty;
}
