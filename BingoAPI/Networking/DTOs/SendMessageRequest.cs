using Newtonsoft.Json;

namespace BingoAPI.Networking.DTOs;

/// <summary>
/// Model used as the request's payload of <see cref="BingoApiClient.SendMessage"/>
/// </summary>
internal record SendMessageRequest
{
	[JsonProperty("room")]
	public string Code = string.Empty;

	[JsonProperty("text")]
	public string Message = string.Empty;
}
