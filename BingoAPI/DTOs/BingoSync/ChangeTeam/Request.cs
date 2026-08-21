using BingoAPI.Clients.BingoSync;
using BingoAPI.Models;
using Newtonsoft.Json;

namespace BingoAPI.DTOs.BingoSync.ChangeTeam;

/// <summary>
/// Model used as the payload of <see cref="BingoSyncApiClient.ChangeTeam(string,Models.Team,CancellationToken)"/>
/// </summary>
internal sealed record Request
{
	[JsonProperty("room")]
	[JsonRequired]
	public required string Code { get; init; }

	[JsonProperty("color")]
	[JsonRequired]
	public required Team Team { get; init; }
}
