using System.Threading.Tasks;
using BingoAPI.Entities.Events;
using BingoAPI.Extensions;
using BingoAPI.Helpers;
using BingoAPI.Managers;
using BingoAPI.Models;

namespace BingoAPI.Entities.Clients;

/// <summary>
/// Client that converts received <see cref="BaseEvent"/> into calls
/// </summary>
/// <remarks>
/// You can subscribe to <see cref="EventManager"/> to get notify about this client's events
/// </remarks>
public class EventClient : BaseClient
{
    #region Events

    /// <inheritdoc/>
    protected override void OnEvent(BaseEvent baseEvent)
    {
        switch (baseEvent)
        {
            case ConnectedEvent _connected:
                OnConnectedEvent(_connected);
                break;
            case DisconnectedEvent _disconnected:
                OnDisconnectedEvent(_disconnected);
                break;
            case ChatEvent _chat:
                OnChatEvent(_chat);
                break;
            case ColorEvent _color:
                OnColorEvent(_color);
                break;
            case GoalEvent _goal:
                if (_goal.HasBeenCleared)
                    OnGoalCleared(_goal);
                else
                    OnGoalMarked(_goal);
                break;
        }
    }

    private void OnConnectedEvent(ConnectedEvent @event)
    {
        if (IsInRoom)
        {
            OnOtherConnect(@event.RoomId, @event.Player);
            EventManager.OnOtherConnected.Invoke(@event.RoomId, @event.Player);
            return;
        }
        
        OnSelfConnect(@event.RoomId, @event.Player);
        EventManager.OnSelfConnected.Invoke(@event.RoomId, @event.Player);
    }

    private void OnDisconnectedEvent(DisconnectedEvent @event)
    {
        if (!IsInRoom)
        {
            Log.Warning("Receiving a disconnecting event without being in a room.");
            return;
        }
        
        OnOtherDisconnect(@event.RoomId, @event.Player);
        EventManager.OnOtherDisconnected.Invoke(@event.RoomId, @event.Player);
    }

    private void OnChatEvent(ChatEvent @event)
    {
        if (@event.IsFromLocal(this))
        {
            OnSelfMessageReceived(@event.Text, @event.Timestamp);
            EventManager.OnSelfChatted.Invoke(@event.Player, @event.Text, @event.Timestamp);
            return;
        }

        OnOtherMessageReceived(@event.Player, @event.Text, @event.Timestamp);
        EventManager.OnOtherChatted.Invoke(@event.Player, @event.Text, @event.Timestamp);
    }
    
    private void OnColorEvent(ColorEvent @event)
    {
        if (@event.IsFromLocal(this))
        {
            OnSelfTeamChange(@event.Player.Team);
            EventManager.OnSelfTeamChanged.Invoke(@event.Player, @event.Player.Team);
            return;
        }

        OnOtherTeamChange(@event.Player, @event.Player.Team);
        EventManager.OnOtherTeamChanged.Invoke(@event.Player, @event.Player.Team);
    }
    
    private void OnGoalCleared(GoalEvent @event)
    {
        if (@event.IsFromLocal(this))
        {
            OnSelfClear(@event.Square);
            EventManager.OnSelfCleared.Invoke(@event.Player, @event.Square);
            return;
        }

        OnOtherClear(@event.Player, @event.Square);
        EventManager.OnOtherCleared.Invoke(@event.Player, @event.Square);
    }
    
    private void OnGoalMarked(GoalEvent @event)
    {
        if (@event.IsFromLocal(this))
        {
            OnSelfMark(@event.Square);
            EventManager.OnSelfMarked.Invoke(@event.Player, @event.Square);
            return;
        }

        OnOtherMark(@event.Player, @event.Square);
        EventManager.OnOtherMarked.Invoke(@event.Player, @event.Square);
    }
    
    #endregion

    #region Callbacks
    
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
        EventManager.OnSelfDisconnected.Invoke();
        return true;
    }

    #endregion
}