using BingoAPI.Models;

namespace BingoAPI.Events.BingoSync;

/// <summary>
/// Event sent when a player sends a message in the room
/// </summary>
internal record ChatEvent : IEvent
{
	/// <summary>
	/// Player responsible for this event
	/// </summary>
	public required Player Player { get; init; }

	/// <summary>
	/// Time when this event was sent
	/// </summary>
	public required ulong Timestamp { get; init; }

	/// <summary>
	/// Content of the message sent
	/// </summary>
	public required string Text { get; init; }
}
