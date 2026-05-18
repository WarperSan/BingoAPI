using BingoAPI.Models;
using Newtonsoft.Json;

namespace BingoAPI.Networking.DTOs;

/// <summary>
/// Model used as the request's payload of <see cref="BingoApiClient.ChangeTeam"/>
/// </summary>
internal record ChangeTeamRequest
{
	[JsonProperty("room")]
	public string Code = string.Empty;

	[JsonProperty("color")]
	public Team Team;
}
