using BingoAPI.Clients.BingoSync;
using Newtonsoft.Json;

namespace BingoAPI.DTOs.BingoSync.SendMessage;

/// <summary>
/// Model used as the payload of <see cref="BingoSyncApiClient.SendMessage(string,string,CancellationToken)"/>
/// </summary>
internal sealed record Request
{
	[JsonProperty("room")]
	[JsonRequired]
	public required string Code { get; init; }

	[JsonProperty("text")]
	[JsonRequired]
	public required string Message { get; init; }
}
