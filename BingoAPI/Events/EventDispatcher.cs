using BingoAPI.Events.BuiltIn;
using BingoAPI.Models;
using JetBrains.Annotations;

namespace BingoAPI.Events;

/// <summary>
/// Dispatches incoming <see cref="IEvent"/> into respective callbacks
/// </summary>
[PublicAPI]
public sealed class EventDispatcher
{
	private string? _localUUID;

	/// <summary>
	/// Sets the local player used to differentiate self events from others
	/// </summary>
	internal void SetLocalPlayer(string uuid) => _localUUID = uuid;

	/// <summary>
	/// Checks if the given <see cref="Player"/> is the local player
	/// </summary>
	private bool IsLocal(Player player) => player.UUID == _localUUID;

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

	#region Dispatch

	/// <summary>
	/// Called when a <see cref="IEvent"/> is received
	/// </summary>
	internal void Dispatch(IEvent evt)
	{
		switch (evt)
		{
			case ConnectionEvent connection:
				if (connection.IsConnected)
					DispatchConnectedEvent(connection);
				else
					DispatchDisconnectedEvent(connection);
				break;
			case ChatEvent chat:
				DispatchChatEvent(chat);
				break;
			case ColorEvent color:
				DispatchColorEvent(color);
				break;
			case GoalEvent goal:
				if (goal.HasBeenCleared)
					DispatchGoalCleared(goal);
				else
					DispatchGoalMarked(goal);
				break;
			case CardRevealedEvent reveal:
				DispatchCardRevealed(reveal);
				break;
			case CardGeneratedEvent generate:
				DispatchCardGenerated(generate);
				break;
		}
	}

	private void DispatchConnectedEvent(ConnectionEvent evt)
	{
		if (IsLocal(evt.Player))
			OnSelfConnected?.Invoke(evt.Player);
		else
			OnOtherConnected?.Invoke(evt.Player);
	}

	private void DispatchDisconnectedEvent(ConnectionEvent evt)
	{
		if (IsLocal(evt.Player))
			OnSelfDisconnected?.Invoke(evt.Player);
		else
			OnOtherDisconnected?.Invoke(evt.Player);
	}

	private void DispatchChatEvent(ChatEvent evt)
	{
		if (IsLocal(evt.Player))
			OnSelfMessageSent?.Invoke(evt.Player, evt.Text, evt.Timestamp);
		else
			OnOtherMessageSent?.Invoke(evt.Player, evt.Text, evt.Timestamp);
	}

	private void DispatchColorEvent(ColorEvent evt)
	{
		if (IsLocal(evt.Player))
			OnSelfTeamChanged?.Invoke(evt.Player, evt.NewColor);
		else
			OnOtherTeamChanged?.Invoke(evt.Player, evt.NewColor);
	}

	private void DispatchGoalMarked(GoalEvent evt)
	{
		if (IsLocal(evt.Player))
			OnSelfSquareMarked?.Invoke(evt.Player, evt.Square, evt.Team);
		else
			OnOtherSquareMarked?.Invoke(evt.Player, evt.Square, evt.Team);
	}

	private void DispatchGoalCleared(GoalEvent evt)
	{
		if (IsLocal(evt.Player))
			OnSelfSquareCleared?.Invoke(evt.Player, evt.Square, evt.Team);
		else
			OnOtherSquareCleared?.Invoke(evt.Player, evt.Square, evt.Team);
	}

	private void DispatchCardRevealed(CardRevealedEvent evt)
	{
		if (IsLocal(evt.Player))
			OnSelfCardRevealed?.Invoke(evt.Player);
		else
			OnOtherCardRevealed?.Invoke(evt.Player);
	}

	private void DispatchCardGenerated(CardGeneratedEvent evt)
	{
		if (IsLocal(evt.Player))
			OnSelfCardGenerated?.Invoke(evt.Player, evt.IsCardHidden);
		else
			OnOtherCardGenerated?.Invoke(evt.Player, evt.IsCardHidden);
	}

	#endregion
}
