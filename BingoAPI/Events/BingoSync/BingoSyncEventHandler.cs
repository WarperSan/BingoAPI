using BingoAPI.Logging;
using BingoAPI.Models;

namespace BingoAPI.Events.BingoSync;

/// <summary>
/// Default implementation of <see cref="IEventHandler"/> for BingoSync
/// </summary>
public sealed class BingoSyncEventHandler : IEventHandler, IEventCallback
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

	#region Callbacks

	/// <inheritdoc />
	public event IEventCallback.ConnectionCallback? OnSelfConnected;

	/// <inheritdoc />
	public event IEventCallback.DisconnectionCallback? OnSelfDisconnected;

	/// <inheritdoc />
	public event IEventCallback.MarkCallback? OnSelfSquareMarked;

	/// <inheritdoc />
	public event IEventCallback.ClearCallback? OnSelfSquareCleared;

	/// <inheritdoc />
	public event IEventCallback.ChatCallback? OnSelfMessageSent;

	/// <inheritdoc />
	public event IEventCallback.TeamCallback? OnSelfTeamChanged;

	/// <inheritdoc />
	public event IEventCallback.RevealCallback? OnSelfCardRevealed;

	/// <inheritdoc />
	public event IEventCallback.GenerateCallback? OnSelfCardGenerated;

	/// <inheritdoc />
	public event IEventCallback.ConnectionCallback? OnOtherConnected;

	/// <inheritdoc />
	public event IEventCallback.DisconnectionCallback? OnOtherDisconnected;

	/// <inheritdoc />
	public event IEventCallback.MarkCallback? OnOtherSquareMarked;

	/// <inheritdoc />
	public event IEventCallback.ClearCallback? OnOtherSquareCleared;

	/// <inheritdoc />
	public event IEventCallback.ChatCallback? OnOtherMessageSent;

	/// <inheritdoc />
	public event IEventCallback.TeamCallback? OnOtherTeamChanged;

	/// <inheritdoc />
	public event IEventCallback.RevealCallback? OnOtherCardRevealed;

	/// <inheritdoc />
	public event IEventCallback.GenerateCallback? OnOtherCardGenerated;

	#endregion
}
