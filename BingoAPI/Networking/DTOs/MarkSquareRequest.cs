using BingoAPI.Models;
using Newtonsoft.Json;

namespace BingoAPI.Networking.DTOs;

/// <summary>
/// Model used as the request's payload of <see cref="BingoApiClient.MarkSquare"/>
/// </summary>
public record MarkSquareRequest
{
	[JsonProperty("room")]
	public string Code = string.Empty;

	[JsonProperty("color")]
	public Team Team;

	[JsonProperty("slot")]
	public string Index = string.Empty;

	[JsonProperty("remove_color")]
	public bool RemoveColor => false;
}
