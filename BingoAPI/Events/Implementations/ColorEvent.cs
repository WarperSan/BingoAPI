using BingoAPI.Events.Interfaces;
using BingoAPI.Models;
using Newtonsoft.Json;

namespace BingoAPI.Events.Implementations;

/// <summary>
/// Event sent when a player changes team
/// </summary>
internal record ColorEvent : IColorEvent
{
	/// <inheritdoc/>
	[JsonProperty("player")]
	[JsonRequired]
	public required Player Player { get; init; }

	/// <inheritdoc/>
	[JsonProperty("color")]
	[JsonRequired]
	public required Team NewColor { get; init; }
}
