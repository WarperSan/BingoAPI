using Newtonsoft.Json;

namespace BingoAPI.Networking.DTOs;

/// <summary>
/// Model used as the request's payload of <see cref="BingoApiClient.ChangeTeam"/>
/// </summary>
public record ApiChangeTeamRequest
{
	[JsonProperty("room")]
	public string Code = string.Empty;

	[JsonProperty("color")]
	public string Team = string.Empty;
}
