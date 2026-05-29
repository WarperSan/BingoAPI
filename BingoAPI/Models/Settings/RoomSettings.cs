using BingoAPI.Networking.Clients;
using BingoAPI.Networking.Converters;
using Newtonsoft.Json;

namespace BingoAPI.Models.Settings;

/// <summary>
/// Data returned when calling <see cref="BingoApiClient.GetRoomSettings"/>
/// </summary>
public struct RoomSettings
{
	/// <summary>
	/// Determines if the room is in lockout mode
	/// </summary>
	[JsonProperty("lockout_mode")]
	[JsonRequired]
	[JsonConverter(typeof(StringEqualConverter), "Lockout")]
	public readonly bool IsLockout;
}
