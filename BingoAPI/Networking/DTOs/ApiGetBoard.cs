using BingoAPI.Models;
using Newtonsoft.Json;

namespace BingoAPI.Networking.DTOs;

/// <summary>
/// Model used as the response's items of <see cref="BingoApiClient.GetBoard"/>
/// </summary>
public record ApiGetBoardItem
{
	[JsonProperty("name")]
	[JsonRequired]
	public string Name = string.Empty;

	[JsonProperty("slot")]
	[JsonRequired]
	public string Slot = string.Empty;

	[JsonProperty("colors")]
	[JsonRequired]
	public Team Team;
}

