using BingoAPI.Events.Interfaces;
using BingoAPI.Models;
using Newtonsoft.Json;

namespace BingoAPI.Events.Implementations;

/// <summary>
/// Event sent when a player generates a new card
/// </summary>
internal record CardGeneratedEvent : ICardGeneratedEvent
{
	/// <inheritdoc/>
	[JsonProperty("player")]
	[JsonRequired]
	public required Player Player { get; init; }

	/// <inheritdoc/>
	[JsonProperty("hide_card")]
	[JsonRequired]
	public required bool IsCardHidden { get; init; }
}
