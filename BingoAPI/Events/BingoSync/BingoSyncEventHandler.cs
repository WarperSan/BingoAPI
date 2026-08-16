using BingoAPI.Logging;
using BingoAPI.Models;

namespace BingoAPI.Events.BingoSync;

/// <summary>
/// Default implementation of <see cref="IEventHandler"/> for BingoSync
/// </summary>
public class BingoSyncEventHandler : IEventHandler
{
	private Player? _localPlayer;
	private readonly ILogger _logger;

	/// <summary>
	/// Initializes a new instance of the <see cref="BingoSyncEventHandler"/> class.
	/// </summary>
	public BingoSyncEventHandler(ILogger logger)
	{
		_logger = logger;
	}

	/// <summary>
	/// Checks if the given <see cref="Player"/> is the local player
	/// </summary>
	private bool IsLocal(Player player) => player.UUID == _localPlayer?.UUID;

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

	/// <inheritdoc />
	public void Handle(IEvent evt)
	{
		switch (evt)
		{
			case ConnectionEvent connection:
				if (connection.IsConnected)
					HandleConnectedEvent(connection);
				else
					HandleDisconnectedEvent(connection);
				break;
			case ChatEvent chat:
				HandleChatEvent(chat);
				break;
			case ColorEvent color:
				HandleColorEvent(color);
				break;
			case GoalEvent goal:
				if (goal.HasBeenCleared)
					HandleGoalCleared(goal);
				else
					HandleGoalMarked(goal);
				break;
			case CardRevealedEvent reveal:
				HandleCardRevealed(reveal);
				break;
			case CardGeneratedEvent generate:
				HandleCardGenerated(generate);
				break;
			default:
				_logger.Debug(
					$"Class '{nameof(BingoSyncEventHandler)}' does not handle events of type '{evt.GetType()}'."
				);
				break;
		}
	}

	/// <inheritdoc />
	public void HandleConnect(Player player)
	{
		if (_localPlayer != null)
		{
			_logger.Warning("Tried to handle a connection while still being connected.");
			return;
		}

		if (IsLocal(player))
		{
			_logger.Warning(
				"Tried to handle a connection while being connected for the same player."
			);
			return;
		}

		_localPlayer = player;
		OnSelfConnected?.Invoke(player);
	}

	/// <inheritdoc />
	public void HandleDisconnect()
	{
		if (_localPlayer == null)
		{
			_logger.Warning("Tried to handle a disconnection while still being disconnected.");
			return;
		}

		OnSelfDisconnected?.Invoke(_localPlayer);
		_localPlayer = null;
	}

	#region Events

	private void HandleConnectedEvent(ConnectionEvent evt)
	{
		if (IsLocal(evt.Player))
		{
			_logger.Warning("Received a connection event from the local player.");
			return;
		}

		OnOtherConnected?.Invoke(evt.Player);
	}

	private void HandleDisconnectedEvent(ConnectionEvent evt)
	{
		if (IsLocal(evt.Player))
		{
			_logger.Warning("Received a disconnection event from the local player.");
			return;
		}

		OnOtherDisconnected?.Invoke(evt.Player);
	}

	private void HandleChatEvent(ChatEvent evt)
	{
		if (IsLocal(evt.Player))
			OnSelfMessageSent?.Invoke(evt.Player, evt.Text, evt.Timestamp);
		else
			OnOtherMessageSent?.Invoke(evt.Player, evt.Text, evt.Timestamp);
	}

	private void HandleColorEvent(ColorEvent evt)
	{
		if (IsLocal(evt.Player))
			OnSelfTeamChanged?.Invoke(evt.Player, evt.NewColor);
		else
			OnOtherTeamChanged?.Invoke(evt.Player, evt.NewColor);
	}

	private void HandleGoalMarked(GoalEvent evt)
	{
		if (IsLocal(evt.Player))
			OnSelfSquareMarked?.Invoke(evt.Player, evt.Square, evt.Team);
		else
			OnOtherSquareMarked?.Invoke(evt.Player, evt.Square, evt.Team);
	}

	private void HandleGoalCleared(GoalEvent evt)
	{
		if (IsLocal(evt.Player))
			OnSelfSquareCleared?.Invoke(evt.Player, evt.Square, evt.Team);
		else
			OnOtherSquareCleared?.Invoke(evt.Player, evt.Square, evt.Team);
	}

	private void HandleCardRevealed(CardRevealedEvent evt)
	{
		if (IsLocal(evt.Player))
			OnSelfCardRevealed?.Invoke(evt.Player);
		else
			OnOtherCardRevealed?.Invoke(evt.Player);
	}

	private void HandleCardGenerated(CardGeneratedEvent evt)
	{
		if (IsLocal(evt.Player))
			OnSelfCardGenerated?.Invoke(evt.Player, evt.IsCardHidden);
		else
			OnOtherCardGenerated?.Invoke(evt.Player, evt.IsCardHidden);
	}

	#endregion
}
