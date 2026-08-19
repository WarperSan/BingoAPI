using BingoAPI.Clients.BingoSync;
using Newtonsoft.Json;

namespace BingoAPI.DTOs.BingoSync.RevealCard;

/// <summary>
/// Model used as the payload of <see cref="BingoSyncApiClient.RevealCard(string,CancellationToken)"/>
/// </summary>
internal record Request
{
	[JsonProperty("room")]
	[JsonRequired]
	public required string Code { get; init; }
}
