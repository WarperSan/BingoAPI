using BingoAPI.Events;
using BingoAPI.Extensions;
using BingoAPI.Helpers;
using BingoAPI.Models;

namespace BingoAPI.Clients;

/// <summary>
/// Client that converts received <see cref="BaseEvent"/> into calls
/// </summary>
public class EventClient : BaseClient
{
	#region Events

	/// <inheritdoc/>
	protected override void OnEvent(BaseEvent baseEvent)
	{
		switch (baseEvent)
		{
			case ConnectedEvent connected:
				OnConnectedEvent(connected);
				break;
			case DisconnectedEvent disconnected:
				OnDisconnectedEvent(disconnected);
				break;
			case ChatEvent chat:
				OnChatEvent(chat);
				break;
			case ColorEvent color:
				OnColorEvent(color);
				break;
			case GoalEvent goal:
				if (goal.HasBeenCleared)
					OnGoalCleared(goal);
				else
					OnGoalMarked(goal);
				break;
			default:
				OnUnhandledEvent(baseEvent);
				break;
		}
	}

	private void OnConnectedEvent(ConnectedEvent @event)
	{
		if (@event.IsFromLocal(this))
		{
			OnSelfConnected?.Invoke(@event.RoomId, @event.Player);
			return;
		}

		OnOtherConnected?.Invoke(@event.RoomId, @event.Player);
	}

	private void OnDisconnectedEvent(DisconnectedEvent @event)
	{
		if (!IsInRoom)
		{
			Log.Warning("Receiving a disconnecting event without being in a room.");
			return;
		}

		OnOtherDisconnected?.Invoke(@event.RoomId, @event.Player);
	}

	private void OnChatEvent(ChatEvent @event)
	{
		if (@event.IsFromLocal(this))
		{
			OnSelfChatted?.Invoke(@event.Player, @event.Text, @event.Timestamp);
			return;
		}

		OnOtherChatted?.Invoke(@event.Player, @event.Text, @event.Timestamp);
	}

	private void OnColorEvent(ColorEvent @event)
	{
		if (@event.IsFromLocal(this))
		{
			OnSelfTeamChanged?.Invoke(@event.Player, @event.Player.Team);
			return;
		}

		OnOtherTeamChanged?.Invoke(@event.Player, @event.Player.Team);
	}

	private void OnGoalCleared(GoalEvent @event)
	{
		if (@event.IsFromLocal(this))
		{
			OnSelfCleared?.Invoke(@event.Player, @event.Square);
			return;
		}

		OnOtherCleared?.Invoke(@event.Player, @event.Square);
	}

	private void OnGoalMarked(GoalEvent @event)
	{
		if (@event.IsFromLocal(this))
		{
			OnSelfMarked?.Invoke(@event.Player, @event.Square);
			return;
		}

		OnOtherMarked?.Invoke(@event.Player, @event.Square);
	}

	/// <summary>
	/// Called when a <see cref="BaseEvent"/> was not handled by <see cref="EventClient"/>
	/// </summary>
	protected virtual void OnUnhandledEvent(BaseEvent baseEvent)
	{
	}

	#endregion

	#region Delegates

	public delegate void ConnectionCallback(string? roomId, PlayerData player);

	public delegate void DisconnectionCallback(string? roomId, PlayerData player);

	public delegate void MarkCallback(PlayerData player, SquareData square);

	public delegate void ClearCallback(PlayerData player, SquareData square);

	public delegate void ChatCallback(PlayerData player, string content, ulong timestamp);

	public delegate void TeamCallback(PlayerData player, Team newTeam);

	#endregion

	#region Alias Callbacks

	/// <summary>
	/// Called when any client sends a message
	/// </summary>
	/// <remarks>
	/// This is equivalent to <see cref="OnSelfChatted"/> and <see cref="OnOtherChatted"/>
	/// </remarks>
	public event ChatCallback? OnChatted
	{
		add
		{
			OnSelfChatted += value;
			OnOtherChatted += value;
		}
		remove
		{
			OnSelfChatted -= value;
			OnOtherChatted -= value;
		}
	}

	/// <summary>
	/// Called when any client marks a goal
	/// </summary>
	/// <remarks>
	/// This is equivalent to <see cref="OnSelfMarked"/> and <see cref="OnOtherMarked"/>
	/// </remarks>
	public event MarkCallback? OnMarked
	{
		add
		{
			OnSelfMarked += value;
			OnOtherMarked += value;
		}
		remove
		{
			OnSelfMarked -= value;
			OnOtherMarked -= value;
		}
	}

	/// <summary>
	/// Called when any client clears a goal
	/// </summary>
	/// <remarks>
	/// This is equivalent to <see cref="OnSelfCleared"/> and <see cref="OnOtherCleared"/>
	/// </remarks>
	public event ClearCallback? OnCleared
	{
		add
		{
			OnSelfCleared += value;
			OnOtherCleared += value;
		}
		remove
		{
			OnSelfCleared -= value;
			OnOtherCleared -= value;
		}
	}

	#endregion

	#region Callbacks

	/// <summary>
	/// Called when this client gets connected to a room
	/// </summary>
	public event ConnectionCallback? OnSelfConnected;

	/// <summary>
	/// Called when this client gets disconnected from a room
	/// </summary>
	public event Action? OnSelfDisconnected;

	/// <summary>
	/// Called when this client has marked a square
	/// </summary>
	public event MarkCallback? OnSelfMarked;

	/// <summary>
	/// Called when this client has cleared a square
	/// </summary>
	public event ClearCallback? OnSelfCleared;

	/// <summary>
	/// Called when this client has sent a message in a room
	/// </summary>
	public event ChatCallback? OnSelfChatted;

	/// <summary>
	/// Called when this client has changed team
	/// </summary>
	public event TeamCallback? OnSelfTeamChanged;

	/// <summary>
	/// Called when another client gets connected
	/// </summary>
	public event ConnectionCallback? OnOtherConnected;

	/// <summary>
	/// Called when another client gets disconnected
	/// </summary>
	public event DisconnectionCallback? OnOtherDisconnected;

	/// <summary>
	/// Called when another client has marked a square
	/// </summary>
	public event MarkCallback? OnOtherMarked;

	/// <summary>
	/// Called when another client has cleared a square
	/// </summary>
	public event ClearCallback? OnOtherCleared;

	/// <summary>
	/// Called when another client has sent a message in a room
	/// </summary>
	public event ChatCallback? OnOtherChatted;

	/// <summary>
	/// Called when another client has changed team
	/// </summary>
	public event TeamCallback? OnOtherTeamChanged;

	#endregion

	#region BaseClient

	/// <inheritdoc/>
	protected override async Task<bool> Disconnect()
	{
		var disconnected = await base.Disconnect();

		if (!disconnected)
			return false;

		OnSelfDisconnected?.Invoke();
		return true;
	}

	#endregion
}
