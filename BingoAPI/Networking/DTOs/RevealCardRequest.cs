using Newtonsoft.Json;

namespace BingoAPI.Networking.DTOs;

/// <summary>
/// Model used as the request's payload of <see cref="BingoApiClient.MarkSquare"/>
/// </summary>
internal record RevealCardRequest
{
	[JsonProperty("room")]
	public string Code = string.Empty;
}
