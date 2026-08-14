using BingoAPI.Models;
using Newtonsoft.Json;

namespace BingoAPI.DTOs.ClearSquare;

internal record Request
{
	[JsonProperty("room")]
	public required string Code { get; init; }

	[JsonProperty("color")]
	public required Team Team { get; init; }

	[JsonProperty("slot")]
	public required string Index { get; init; }

	[JsonProperty("remove_color")]
	public bool RemoveColor => true;
}
