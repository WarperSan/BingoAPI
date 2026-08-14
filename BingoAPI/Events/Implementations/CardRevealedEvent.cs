using BingoAPI.Events.Interfaces;
using BingoAPI.Models;
using Newtonsoft.Json;

namespace BingoAPI.Events.Implementations;

/// <summary>
/// Event sent when a player reveals the card
/// </summary>
internal record CardRevealedEvent : ICardRevealedEvent
{
	/// <inheritdoc/>
	[JsonProperty("player")]
	[JsonRequired]
	public required Player Player { get; init; }
}
