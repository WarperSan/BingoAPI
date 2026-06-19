using BingoAPI.Networking.Clients;

namespace BingoAPI.Networking.DTOs;

/// <summary>
/// Model used as the response of <see cref="BingoApiClient.GetTokens"/>
/// </summary>
internal record Tokens
{
	public required string PublicToken { get; init; }
	public required string CreationToken { get; init; }
}
