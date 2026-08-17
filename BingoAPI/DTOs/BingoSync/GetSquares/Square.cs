using BingoAPI.Clients.BingoSync;
using BingoAPI.Models;
using Newtonsoft.Json;

namespace BingoAPI.DTOs.BingoSync.GetSquares;

/// <summary>
/// Model used as part of the response of <see cref="BingoSyncApiClient.GetSquares(string,CancellationToken)"/>
/// </summary>
internal record Square
{
	[JsonProperty("name")]
	[JsonRequired]
	public required string Text { get; init; }

	[JsonProperty("slot")]
	[JsonRequired]
	public required SlotIndex Slot { get; init; }

	[JsonProperty("colors")]
	[JsonRequired]
	public required Team Teams { get; init; }
}
