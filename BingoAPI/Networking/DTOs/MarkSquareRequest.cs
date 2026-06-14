using BingoAPI.Models;
using BingoAPI.Networking.Clients;
using Newtonsoft.Json;

namespace BingoAPI.Networking.DTOs;

/// <summary>
/// Model used as the request's payload of <see cref="BingoApiClient.MarkSquare"/>
/// </summary>
internal record MarkSquareRequest
{
	[JsonProperty("room")]
	public required string Code { get; init; }

	[JsonProperty("color")]
	public required Team Team { get; init; }

	[JsonProperty("slot")]
	public required string Index { get; init; }

	[JsonProperty("remove_color")]
	public bool RemoveColor => false;
}
