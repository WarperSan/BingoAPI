using BingoAPI.Clients.BingoSync;
using BingoAPI.Models;
using Newtonsoft.Json;

namespace BingoAPI.DTOs.BingoSync.ClearSquare;

/// <summary>
/// Model used as the payload of <see cref="BingoSyncApiClient.ClearSquare(string,Models.Team,int,CancellationToken)"/>
/// </summary>
internal class Request
{
	[JsonProperty("room")]
	[JsonRequired]
	public required string Code { get; init; }

	[JsonProperty("color")]
	[JsonRequired]
	public required Team Team { get; init; }

	[JsonProperty("slot")]
	[JsonRequired]
	public required string Index { get; init; }

	[JsonProperty("remove_color")]
	[JsonRequired]
	public bool RemoveColor => true;
}
