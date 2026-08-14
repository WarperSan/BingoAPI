using BingoAPI.Models;
using Newtonsoft.Json;

namespace BingoAPI.DTOs.ChangeTeam;

internal record Request
{
	[JsonProperty("room")]
	public required string Code { get; init; }

	[JsonProperty("color")]
	public required Team Team { get; init; }
}
