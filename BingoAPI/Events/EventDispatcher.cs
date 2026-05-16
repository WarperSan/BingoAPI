using BingoAPI.Models;

namespace BingoAPI.Events;

/// <summary>
/// Dispatches incoming <see cref="IBingoEvent"/> into respective callbacks
/// </summary>
public sealed class EventDispatcher
{
	private string? _localUUID;

	/// <summary>
	/// Sets the local player used to differentiate self events from others
	/// </summary>
	internal void SetLocalPlayer(Player player) => _localUUID = player.UUID;

	/// <summary>
	/// Checks if the given <see cref="IBingoEvent"/> is from the local player
	/// </summary>
	private bool IsFromLocal(Player player) => player.UUID == _localUUID;

	#region Delegates

	public delegate void ConnectionCallback(Player player);

	public delegate void DisconnectionCallback(Player player);

	public delegate void MarkCallback(Player player, Square square);

	public delegate void ClearCallback(Player player, Square square);

	public delegate void ChatCallback(Player player, string content, ulong timestamp);

	public delegate void TeamCallback(Player player, Team newTeam);

	#endregion

	#region Alias Callbacks

	/// <summary>
	/// Called when any player sends a message
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
	/// Called when any player marks a goal
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
	/// Called when any player clears a goal
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
	/// Called when this player gets connected to a room
	/// </summary>
	public event ConnectionCallback? OnSelfConnected;

	/// <summary>
	/// Called when this player gets disconnected from a room
	/// </summary>
	public event Action? OnSelfDisconnected;

	/// <summary>
	/// Called when this player has marked a square
	/// </summary>
	public event MarkCallback? OnSelfMarked;

	/// <summary>
	/// Called when this player has cleared a square
	/// </summary>
	public event ClearCallback? OnSelfCleared;

	/// <summary>
	/// Called when this player has sent a message in a room
	/// </summary>
	public event ChatCallback? OnSelfChatted;

	/// <summary>
	/// Called when this player has changed team
	/// </summary>
	public event TeamCallback? OnSelfTeamChanged;

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
	public event MarkCallback? OnOtherMarked;

	/// <summary>
	/// Called when another player has cleared a square
	/// </summary>
	public event ClearCallback? OnOtherCleared;

	/// <summary>
	/// Called when another player has sent a message in a room
	/// </summary>
	public event ChatCallback? OnOtherChatted;

	/// <summary>
	/// Called when another player has changed team
	/// </summary>
	public event TeamCallback? OnOtherTeamChanged;

	#endregion

	#region Dispatch

	/// <summary>
	/// Called when a <see cref="IBingoEvent"/> is received
	/// </summary>
	internal void Dispatch(IBingoEvent evt)
	{
		switch (evt)
		{
			case ConnectionEvent connection:
				if (connection.IsConnected)
					OnConnectedEvent(connection);
				else
					OnDisconnectedEvent(connection);
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

	private void OnConnectedEvent(ConnectionEvent evt)
	{
		if (IsFromLocal(evt.Player))
			OnSelfConnected?.Invoke(evt.Player);
		else
			OnOtherConnected?.Invoke(evt.Player);
	}

	private void OnDisconnectedEvent(ConnectionEvent evt)
	{
		if (IsFromLocal(evt.Player))
			OnSelfDisconnected?.Invoke();
		else
			OnOtherDisconnected?.Invoke(evt.Player);
	}

	private void OnChatEvent(ChatEvent evt)
	{
		if (IsFromLocal(evt.Player))
			OnSelfChatted?.Invoke(evt.Player, evt.Text, evt.Timestamp);
		else
			OnOtherChatted?.Invoke(evt.Player, evt.Text, evt.Timestamp);
	}

	private void OnColorEvent(ColorEvent evt)
	{
		if (IsFromLocal(evt.Player))
			OnSelfTeamChanged?.Invoke(evt.Player, evt.Player.Team);
		else
			OnOtherTeamChanged?.Invoke(evt.Player, evt.Player.Team);
	}

	private void OnGoalCleared(GoalEvent evt)
	{
		if (IsFromLocal(evt.Player))
			OnSelfCleared?.Invoke(evt.Player, evt.Square);
		else
			OnOtherCleared?.Invoke(evt.Player, evt.Square);
	}

	private void OnGoalMarked(GoalEvent evt)
	{
		if (IsFromLocal(evt.Player))
			OnSelfMarked?.Invoke(evt.Player, evt.Square);
		else
			OnOtherMarked?.Invoke(evt.Player, evt.Square);
	}

	#endregion
}
