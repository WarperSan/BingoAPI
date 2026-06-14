using BingoAPI.Models;
using BingoAPI.Networking.Clients;
using Newtonsoft.Json;

namespace BingoAPI.Networking.DTOs;

/// <summary>
/// Model used as the request's payload of <see cref="BingoApiClient.ChangeTeam"/>
/// </summary>
internal record ChangeTeamRequest
{
	[JsonProperty("room")]
	public required string Code { get; init; }

	[JsonProperty("color")]
	public required Team Team { get; init; }
}
