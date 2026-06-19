using BingoAPI.Networking.Clients;
using Newtonsoft.Json;

namespace BingoAPI.Networking.DTOs;

/// <summary>
/// Model used as the request's payload of <see cref="BingoApiClient.JoinRoom"/>
/// </summary>
internal record JoinRoomRequest
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
