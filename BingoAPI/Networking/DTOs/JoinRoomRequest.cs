using Newtonsoft.Json;

namespace BingoAPI.Networking.DTOs;

/// <summary>
/// Model used as the request's payload of <see cref="BingoApiClient.JoinRoom"/>
/// </summary>
public record JoinRoomRequest
{
	[JsonProperty("room")]
	public string Code = string.Empty;

	[JsonProperty("password")]
	public string Password = string.Empty;

	[JsonProperty("nickname")]
	public string Username = string.Empty;

	[JsonProperty("is_spectator")]
	public bool IsSpectator;
}
