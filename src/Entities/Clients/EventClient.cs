using BingoAPI.Entities.Events;
using BingoAPI.Extensions;
using BingoAPI.Helpers;
using BingoAPI.Models;

namespace BingoAPI.Entities.Clients;

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
		}
	}

	private void OnConnectedEvent(ConnectedEvent @event)
	{
		if (IsInRoom)
		{
			OnOtherConnect(@event.RoomId, @event.Player);
			OnOtherConnected?.Invoke(@event.RoomId, @event.Player);
			return;
		}

		OnSelfConnect(@event.RoomId, @event.Player);
		OnSelfConnected?.Invoke(@event.RoomId, @event.Player);
	}

	private void OnDisconnectedEvent(DisconnectedEvent @event)
	{
		if (!IsInRoom)
		{
			Log.Warning("Receiving a disconnecting event without being in a room.");
			return;
		}

		OnOtherDisconnect(@event.RoomId, @event.Player);
		OnOtherDisconnected?.Invoke(@event.RoomId, @event.Player);
	}

	private void OnChatEvent(ChatEvent @event)
	{
		if (@event.IsFromLocal(this))
		{
			OnSelfMessageReceived(@event.Text, @event.Timestamp);
			OnSelfChatted?.Invoke(@event.Player, @event.Text, @event.Timestamp);
			return;
		}

		OnOtherMessageReceived(@event.Player, @event.Text, @event.Timestamp);
		OnOtherChatted?.Invoke(@event.Player, @event.Text, @event.Timestamp);
	}

	private void OnColorEvent(ColorEvent @event)
	{
		if (@event.IsFromLocal(this))
		{
			OnSelfTeamChange(@event.Player.Team);
			OnSelfTeamChanged?.Invoke(@event.Player, @event.Player.Team);
			return;
		}

		OnOtherTeamChange(@event.Player, @event.Player.Team);
		OnOtherTeamChanged?.Invoke(@event.Player, @event.Player.Team);
	}

	private void OnGoalCleared(GoalEvent @event)
	{
		if (@event.IsFromLocal(this))
		{
			OnSelfClear(@event.Square);
			OnSelfCleared?.Invoke(@event.Player, @event.Square);
			return;
		}

		OnOtherClear(@event.Player, @event.Square);
		OnOtherCleared?.Invoke(@event.Player, @event.Square);
	}

	private void OnGoalMarked(GoalEvent @event)
	{
		if (@event.IsFromLocal(this))
		{
			OnSelfMark(@event.Square);
			OnSelfMarked?.Invoke(@event.Player, @event.Square);
			return;
		}

		OnOtherMark(@event.Player, @event.Square);
		OnOtherMarked?.Invoke(@event.Player, @event.Square);
	}

	#endregion

	#region External Callbacks

	/// <summary>
	/// Called when the local client gets connected
	/// </summary>
	public event Action<string?, PlayerData>? OnSelfConnected;

	/// <summary>
	/// Called when the local client gets disconnected
	/// </summary>
	public event Action? OnSelfDisconnected;

	/// <summary>
	/// Called when the local client marks a goal
	/// </summary>
	public event Action<PlayerData, SquareData>? OnSelfMarked;

	/// <summary>
	/// Called when the local client clears a goal
	/// </summary>
	public event Action<PlayerData, SquareData>? OnSelfCleared;

	/// <summary>
	/// Called when the local client sends a message
	/// </summary>
	public event Action<PlayerData, string, ulong>? OnSelfChatted;

	/// <summary>
	/// Called when the local client changes team
	/// </summary>
	public event Action<PlayerData, Team>? OnSelfTeamChanged;

	/// <summary>
	/// Called when another client gets connected
	/// </summary>
	public event Action<string?, PlayerData>? OnOtherConnected;

	/// <summary>
	/// Called when another client gets disconnected
	/// </summary>
	public event Action<string?, PlayerData>? OnOtherDisconnected;

	/// <summary>
	/// Called when another client marks a goal
	/// </summary>
	public event Action<PlayerData, SquareData>? OnOtherMarked;

	/// <summary>
	/// Called when another client clears a goal
	/// </summary>
	public event Action<PlayerData, SquareData>? OnOtherCleared;

	/// <summary>
	/// Called when another client sends a message
	/// </summary>
	public event Action<PlayerData, string, ulong>? OnOtherChatted;

	/// <summary>
	/// Called when another client changes team
	/// </summary>
	public event Action<PlayerData, Team>? OnOtherTeamChanged;

	#endregion

	#region Internal Callbacks

	/// <summary>
	/// Invoked after this client has connected to the room.
	/// </summary>
	protected virtual void OnSelfConnect(string? roomId, PlayerData player) { }

	/// <summary>
	/// Invoked after another client has connected to the room.
	/// </summary>
	protected virtual void OnOtherConnect(string? roomId, PlayerData player) { }

	/// <summary>
	/// Invoked after this client has disconnected from the room.
	/// </summary>
	protected virtual void OnSelfDisconnect() { }

	/// <summary>
	/// Invoked after another client has disconnected from the room.
	/// </summary>
	protected virtual void OnOtherDisconnect(string? roomId, PlayerData player) { }

	/// <summary>
	/// Invoked after this client has marked a square.
	/// </summary>
	protected virtual void OnSelfMark(SquareData square) { }

	/// <summary>
	/// Invoked after another client has marked a square.
	/// </summary>
	protected virtual void OnOtherMark(PlayerData player, SquareData square) { }

	/// <summary>
	/// Invoked after this client has cleared a square.
	/// </summary>
	protected virtual void OnSelfClear(SquareData square) { }

	/// <summary>
	/// Invoked after another client has cleared a square.
	/// </summary>
	protected virtual void OnOtherClear(PlayerData player, SquareData square) { }

	/// <summary>
	/// Invoked after this client has sent a message in the room.
	/// </summary>
	protected virtual void OnSelfMessageReceived(string content, ulong timestamp) { }

	/// <summary>
	/// Invoked after another client has sent a message in the room.
	/// </summary>
	protected virtual void OnOtherMessageReceived(PlayerData player, string content, ulong timestamp) { }

	/// <summary>
	/// Invoked after this client has changed team.
	/// </summary>
	protected virtual void OnSelfTeamChange(Team newTeam) { }

	/// <summary>
	/// Invoked after another client has changed team.
	/// </summary>
	protected virtual void OnOtherTeamChange(PlayerData player, Team newTeam) { }

	#endregion

	#region BaseClient

	/// <inheritdoc/>
	protected override async Task<bool> Disconnect()
	{
		var disconnected = await base.Disconnect();

		if (!disconnected)
			return false;

		OnSelfDisconnect();
		OnSelfDisconnected?.Invoke();
		return true;
	}

	#endregion
}
