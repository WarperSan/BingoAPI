using BingoAPI.Models;

namespace BingoAPI.Events.Interfaces;

/// <summary>
/// Represents any event sent when a player joins or leaves the room
/// </summary>
public interface IConnectionEvent : IEvent
{
	/// <summary>
	/// Player responsible for this event
	/// </summary>
	public Player Player { get; }

	/// <summary>
	/// Defines if the player has connected or disconnected
	/// </summary>
	public bool IsConnected { get; }
}
