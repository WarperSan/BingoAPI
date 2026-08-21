using BingoAPI.Clients.BingoSync;
using Newtonsoft.Json;

namespace BingoAPI.DTOs.BingoSync.GetSocketIdentity;

/// <summary>
/// Model used as the response of <see cref="BingoSyncApiClient.GetSocketIdentity(string,CancellationToken)"/>
/// </summary>
internal sealed record Response
{
	[JsonProperty("room")]
	[JsonRequired]
	public required string Code { get; init; }

	[JsonProperty("player")]
	[JsonRequired]
	public required string PlayerUUID { get; init; }
}
