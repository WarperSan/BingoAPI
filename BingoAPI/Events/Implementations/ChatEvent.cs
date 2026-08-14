using BingoAPI.Events.Interfaces;
using BingoAPI.Models;
using Newtonsoft.Json;

namespace BingoAPI.Events.Implementations;

/// <summary>
/// Event sent when a player sends a message in the room
/// </summary>
internal record ChatEvent : IChatEvent
{
	/// <inheritdoc/>
	[JsonProperty("player")]
	[JsonRequired]
	public required Player Player { get; init; }

	/// <inheritdoc/>
	[JsonProperty("timestamp")]
	[JsonRequired]
	public required ulong Timestamp { get; init; }

	/// <inheritdoc/>
	[JsonProperty("text")]
	[JsonRequired]
	public required string Text { get; init; }
}
