using BingoAPI.Models;
using JetBrains.Annotations;

namespace BingoAPI.Events;

/// <summary>
/// Interface that represents any class that triggers callbacks upon receiving a <see cref="IEvent"/>
/// </summary>
[PublicAPI]
public interface IEventCallback
{
	#region Delegates

	/// <summary>
	/// Callback used when a <see cref="Player"/> connects to a room
	/// </summary>
	public delegate void ConnectionCallback(Player player);

	/// <summary>
	/// Callback used when a <see cref="Player"/> disconnects from a room
	/// </summary>
	public delegate void DisconnectionCallback(Player player);

	/// <summary>
	/// Callback used when a <see cref="Player"/> marks a <see cref="Square"/> for a <see cref="Team"/>
	/// </summary>
	public delegate void MarkCallback(Player player, Square square, Team team);

	/// <summary>
	/// Callback used when a <see cref="Player"/> clears a <see cref="Square"/> for a <see cref="Team"/>
	/// </summary>
	public delegate void ClearCallback(Player player, Square square, Team team);

	/// <summary>
	/// Callback used when a <see cref="Player"/> sends a message in the chat
	/// </summary>
	public delegate void ChatCallback(Player player, string message, ulong timestamp);

	/// <summary>
	/// Callback used when a <see cref="Player"/> changes their <see cref="Team"/>
	/// </summary>
	public delegate void TeamCallback(Player player, Team newTeam);

	/// <summary>
	/// Callback used when a <see cref="Player"/> reveals their card
	/// </summary>
	public delegate void RevealCallback(Player player);

	/// <summary>
	/// Callback used when a <see cref="Player"/> generates a new card
	/// </summary>
	public delegate void GenerateCallback(Player player, bool isHidden);

	#endregion

	#region Callbacks

	/// <summary>
	/// Called when this player gets connected to a room
	/// </summary>
	public event ConnectionCallback? OnSelfConnected;

	/// <summary>
	/// Called when this player gets disconnected from a room
	/// </summary>
	public event DisconnectionCallback? OnSelfDisconnected;

	/// <summary>
	/// Called when this player has marked a square
	/// </summary>
	public event MarkCallback? OnSelfSquareMarked;

	/// <summary>
	/// Called when this player has cleared a square
	/// </summary>
	public event ClearCallback? OnSelfSquareCleared;

	/// <summary>
	/// Called when this player has sent a message in a room
	/// </summary>
	public event ChatCallback? OnSelfMessageSent;

	/// <summary>
	/// Called when this player has changed team
	/// </summary>
	public event TeamCallback? OnSelfTeamChanged;

	/// <summary>
	/// Called when this player has revealed the card
	/// </summary>
	public event RevealCallback? OnSelfCardRevealed;

	/// <summary>
	/// Called when this player has generated a new card
	/// </summary>
	public event GenerateCallback? OnSelfCardGenerated;

	/// <summary>
	/// Called when another player gets connected
	/// </summary>
	public event ConnectionCallback? OnOtherConnected;

	/// <summary>
	/// Called when another player gets disconnected
	/// </summary>
	public event DisconnectionCallback? OnOtherDisconnected;

	/// <summary>
	/// Called when another player has marked a square
	/// </summary>
	public event MarkCallback? OnOtherSquareMarked;

	/// <summary>
	/// Called when another player has cleared a square
	/// </summary>
	public event ClearCallback? OnOtherSquareCleared;

	/// <summary>
	/// Called when another player has sent a message in a room
	/// </summary>
	public event ChatCallback? OnOtherMessageSent;

	/// <summary>
	/// Called when another player has changed team
	/// </summary>
	public event TeamCallback? OnOtherTeamChanged;

	/// <summary>
	/// Called when another player has revealed the card
	/// </summary>
	public event RevealCallback? OnOtherCardRevealed;

	/// <summary>
	/// Called when another player has generated a new card
	/// </summary>
	public event GenerateCallback? OnOtherCardGenerated;

	#endregion
}
