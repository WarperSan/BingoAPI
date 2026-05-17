using BingoAPI.Models.Settings;
using Newtonsoft.Json;

namespace BingoAPI.Networking.DTOs;

/// <summary>
/// Model used as the response's payload of <see cref="BingoApiClient.GetRoomSettings"/>
/// </summary>
public record GetRoomSettingsResponse
{
	[JsonProperty("settings")]
	[JsonRequired]
	public RoomSettings Settings { get; set; }
}
