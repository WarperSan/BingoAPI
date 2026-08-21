using BingoAPI.Clients.BingoSync;

namespace BingoAPI.DTOs.BingoSync.CreateRoom;

/// <summary>
/// Model used as the response of <see cref="BingoSyncApiClient.GetTokens(CancellationToken)"/>
/// </summary>
internal sealed record Tokens
{
	public required string PublicToken { get; init; }
	public required string CreationToken { get; init; }
}
