using Newtonsoft.Json;

namespace BingoAPI.DTOs.JoinRoom;

internal record Request
{
	[JsonProperty("room")]
	public required string Code { get; init; }

	[JsonProperty("password")]
	public required string Password { get; init; }

	[JsonProperty("nickname")]
	public required string Username { get; init; }

	// TODO: Make this a parameter

	[JsonProperty("is_spectator")]
	public bool IsSpectator => false;
}
