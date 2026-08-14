using BingoAPI.Events.Interfaces;
using BingoAPI.Models;
using BingoAPI.Networking.Converters;
using Newtonsoft.Json;

namespace BingoAPI.Events.Implementations;

/// <summary>
/// Event sent when a player joins or leaves the room
/// </summary>
internal record ConnectionEvent : IConnectionEvent
{
	/// <inheritdoc/>
	[JsonProperty("player")]
	[JsonRequired]
	public required Player Player { get; init; }

	/// <inheritdoc/>
	[JsonProperty("event_type")]
	[JsonConverter(typeof(StringEqualConverter), "connected")]
	public required bool IsConnected { get; init; }
}
