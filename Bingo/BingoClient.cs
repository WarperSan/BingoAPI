using System.Net.WebSockets;
using System.Threading.Tasks;
using BingoAPI.Extensions;
using BingoAPI.Helpers;
using BingoAPI.Managers;
using BingoAPI.Models;
using BingoAPI.Models.Events;

namespace BingoAPI.Bingo;

public abstract class BingoClient : Client
{
    public string? roomId { get; private set; }
    public PlayerData PlayerData { get; protected set; } = new()
    {
        UUID = null,
        Name = null,
        Team = Team.BLANK,
        IsSpectator = true
    };

    #region Events

    protected override void HandleEvent(Event @event)
    {
        switch (@event)
        {
            case ConnectedEvent _connected:
                HandleConnectedEvent(_connected);
                break;
            case DisconnectedEvent _disconnected:
                HandleDisconnectedEvent(_disconnected);
                break;
            case ChatEvent _chat:
                HandleChatEvent(_chat);
                break;
            case ColorEvent _color:
                HandleColorEvent(_color);
                break;
            case GoalEvent _goal:
                if (_goal.Remove)
                    HandleClearedEvent(_goal);
                else
                    HandleMarkedEvent(_goal);
                break;
        }
    }

    private void HandleConnectedEvent(ConnectedEvent @event)
    {
        if (roomId == null)
        {
            OnSelfConnect(@event.RoomId, @event.Player);
            
            roomId = @event.RoomId;
            PlayerData = @event.Player;
            
            ClientEventManager.OnSelfConnected.Invoke(@event.RoomId, @event.Player);
        }
        else
        {
            OnOtherConnect(@event.RoomId, @event.Player);
            ClientEventManager.OnOtherConnected.Invoke(@event.RoomId, @event.Player);
        }
    }

    private void HandleDisconnectedEvent(DisconnectedEvent @event)
    {
        if (roomId != null)
        {
            OnOtherDisconnect(@event.RoomId, @event.Player);
            ClientEventManager.OnOtherDisconnected.Invoke(@event.RoomId, @event.Player);
        }
    }
    
    private void HandleChatEvent(ChatEvent @event)
    {
        if (PlayerData.UUID == @event.Player.UUID)
        {
            OnSelfMessageReceived(@event.Text, @event.Timestamp);
            ClientEventManager.OnSelfChatted.Invoke(@event.Player, @event.Text, @event.Timestamp);
        }
        else
        {
            OnOtherMessageReceived(@event.Player, @event.Text, @event.Timestamp);
            ClientEventManager.OnOtherChatted.Invoke(@event.Player, @event.Text, @event.Timestamp);
        }
    }

    private void HandleColorEvent(ColorEvent @event)
    {
        var oldTeam = PlayerData.Team;

        if (PlayerData.UUID == @event.Player.UUID)
        {
            OnSelfTeamChange(oldTeam, @event.Player.Team);
            
            var data = PlayerData;
            data.Team = @event.Player.Team;
            PlayerData = data;
            
            ClientEventManager.OnSelfTeamChanged.Invoke(@event.Player, oldTeam, @event.Player.Team);
        }
        else
        {
            OnOtherTeamChange(@event.Player, oldTeam, @event.Player.Team);
            ClientEventManager.OnOtherTeamChanged.Invoke(@event.Player, oldTeam, @event.Player.Team);
        }
    }

    private void HandleMarkedEvent(GoalEvent @event)
    {
        if (PlayerData.UUID == @event.Player.UUID)
        {
            OnSelfMark(@event.Square);
            ClientEventManager.OnSelfMarked.Invoke(@event.Player, @event.Square);
        }
        else
        {
            OnOtherMark(@event.Player, @event.Square);
            ClientEventManager.OnOtherMarked.Invoke(@event.Player, @event.Square);
        }
    }

    private void HandleClearedEvent(GoalEvent @event)
    {
        if (PlayerData.UUID == @event.Player.UUID)
        {
            OnSelfClear(@event.Square);
            ClientEventManager.OnSelfCleared.Invoke(@event.Player, @event.Square);
        }
        else
        {
            OnOtherClear(@event.Player, @event.Square);
            ClientEventManager.OnOtherCleared.Invoke(@event.Player, @event.Square);
        }
    }
    
    #endregion

    #region Callbacks

    /// <summary>
    /// Called when this client gets connected to the room
    /// </summary>
    protected virtual void OnSelfConnect(string? _roomId, PlayerData player) { }

    /// <summary>
    /// Called when another client gets connected to the room
    /// </summary>
    protected virtual void OnOtherConnect(string? _roomId, PlayerData player) {  }

    /// <summary>
    /// Called when this client gets disconnected to the room
    /// </summary>
    protected virtual void OnSelfDisconnect() { }
    
    /// <summary>
    /// Called when another client gets disconnected to the room
    /// </summary>
    protected virtual void OnOtherDisconnect(string? _roomId, PlayerData player) {  }

    /// <summary>
    /// Called when this client marks a square
    /// </summary>
    protected virtual void OnSelfMark(SquareData square) {  }

    /// <summary>
    /// Called when another client marks a square
    /// </summary>
    protected virtual void OnOtherMark(PlayerData player, SquareData square) {  }

    /// <summary>
    /// Called when this client clears a square
    /// </summary>
    protected virtual void OnSelfClear(SquareData square) {  }

    /// <summary>
    /// Called when another client clears a square
    /// </summary>
    protected virtual void OnOtherClear(PlayerData player, SquareData square) {  }
    
    /// <summary>
    /// Called when this client sends a message to the room
    /// </summary>
    protected virtual void OnSelfMessageReceived(string content, ulong timestamp) {  }
    
    /// <summary>
    /// Called when another client sends a message to the room
    /// </summary>
    protected virtual void OnOtherMessageReceived(PlayerData player, string content, ulong timestamp) { }

    /// <summary>
    /// Called when this client changes team
    /// </summary>
    protected virtual void OnSelfTeamChange(Team oldTeam, Team newTeam) { }
    
    /// <summary>
    /// Called when another client changes team
    /// </summary>
    protected virtual void OnOtherTeamChange(PlayerData player, Team oldTeam, Team newTeam) {  }
    
    #endregion

    #region API

    public async Task<SquareData[]?> GetBoard()
    {
        if (roomId == null)
        {
            Logger.Error("Tried to obtain the board before being connected.");
            return null;
        }

        return await API.GetBoard(roomId);
    }
    
    public async Task ChangeTeam(Team newTeam)
    {
        if (roomId == null)
        {
            Logger.Error("Tried to change team before being connected.");
            return;
        }

        if (!await API.ChangeTeam(roomId, newTeam))
            return;

        var data = PlayerData;
        data.Team = newTeam;
        PlayerData = data;
    }

    public async Task<bool> MarkSquare(int id)
    {
        if (roomId == null)
        {
            Logger.Error("Tried to mark a square before being connected.");
            return false;
        }
        
        return await API.MarkSquare(roomId, PlayerData.Team, id);
    }
    
    public async Task<bool> ClearSquare(int id)
    {
        if (roomId == null)
        {
            Logger.Error("Tried to clear a square before being connected.");
            return false;
        }
        
        return await API.ClearSquare(roomId, PlayerData.Team, id);
    }

    public async Task<bool> SendMessage(string message)
    {
        if (roomId == null)
        {
            Logger.Error("Tried to send a message before being connected.");
            return false;
        }

        return await API.SendMessage(roomId, message);
    }
    
    #endregion

    #region Client

    /// <inheritdoc/>
    internal override async Task<bool> Connect(ClientWebSocket socket)
    {
        if (roomId != null)
            return false;

        if (!await base.Connect(socket))
            return false;

        return await RequestExtension.HandleTimeout(() => roomId != null);
    }

    /// <inheritdoc/>
    public override void Disconnect()
    {
        if (roomId == null)
            return;
        
        Logger.Debug($"Disconnecting client for the room '{roomId}'...");

        base.Disconnect();
        
        roomId = null;
        PlayerData = new PlayerData
        {
            UUID = null,
            Name = null,
            Team = Team.BLANK,
            IsSpectator = true
        };

        OnSelfDisconnect();
        ClientEventManager.OnSelfDisconnected.Invoke();
        
        Logger.Debug("Client disconnected!");
    }

    #endregion
}