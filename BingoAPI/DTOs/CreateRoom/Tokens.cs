namespace BingoAPI.DTOs.CreateRoom;

internal record Tokens
{
	public required string PublicToken { get; init; }
	public required string CreationToken { get; init; }
}
