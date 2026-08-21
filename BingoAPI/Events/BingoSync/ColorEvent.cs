using BingoAPI.Models;
using Newtonsoft.Json;

namespace BingoAPI.Events.BingoSync;

/// <summary>
/// Event sent when a player changes team
/// </summary>
internal sealed record ColorEvent : IEvent
{
	/// <summary>
	/// Player responsible for this event
	/// </summary>
	[JsonProperty("player")]
	[JsonRequired]
	public required Player Player { get; init; }

	/// <summary>
	/// New color of the player
	/// </summary>
	[JsonProperty("color")]
	[JsonRequired]
	public required Team NewColor { get; init; }
}
