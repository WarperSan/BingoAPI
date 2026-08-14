using BingoAPI.Models;

namespace BingoAPI.Events.Interfaces;

/// <summary>
/// Represents any event sent when a player sends a message in the room
/// </summary>
public interface IChatEvent : IEvent
{
	/// <summary>
	/// Player responsible for this event
	/// </summary>
	public Player Player { get; }

	/// <summary>
	/// Time when this event was sent
	/// </summary>
	public ulong Timestamp { get; }

	/// <summary>
	/// Content of the message sent
	/// </summary>
	public string Text { get; }
}
