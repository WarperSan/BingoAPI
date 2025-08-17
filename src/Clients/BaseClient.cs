using System;
using System.Collections.Generic;
using System.Net.WebSockets;
using System.Threading;
using System.Threading.Tasks;
using BingoAPI.Extensions;
using BingoAPI.Helpers;
using BingoAPI.Models;
using BingoAPI.Models.Events;
using BingoAPI.Models.Settings;
using BingoAPI.Network;
using Newtonsoft.Json.Linq;

namespace BingoAPI.Clients;

/// <summary>
/// Class that represents the bare minimum for a client
/// </summary>
public abstract class BaseClient
{
    private const string SOCKETS_URL = "wss://sockets.bingosync.com/broadcast";
    private const string BINGO_URL = "https://bingosync.com";
    
    //public const string NEW_CARD_URL = BINGO_URL + "/api/new-card";
    
    /// <summary>
    /// Current room ID of this client
    /// </summary>
    protected string? RoomID { get; set; }

    /// <summary>
    /// Checks if this client is in a room
    /// </summary>
    protected bool IsInRoom => RoomID != null;
    
    /// <summary>
    /// Current UUID of this player
    /// </summary>
    public string? UUID { get; protected set; }
    
    #region API
    
    /// <summary>
    /// Creates a room with the given settings
    /// </summary>
    /// <returns>Code of the room or null if the room couldn't be created</returns>
    public async Task<string?> CreateRoom(CreateRoomSettings settings)
    {
        var token = await Request.GetCORSToken(BINGO_URL);

        if (token == null)
        {
            Logger.Error("CORS Token not found.");
            return null;
        }
        
        var body = new
        {
            room_name = settings.Name,
            passphrase = settings.Password,
            nickname = PluginInfo.PLUGIN_GUID,
            game_type = 18, // Custom (Advanced)
            variant_type = settings.VariantType,
            custom_json = settings.Goals.GenerateJSON(),
            lockout_mode = settings.LockoutMode,
            seed = settings.Seed,
            is_spectator = settings.IsSpectator,
            hide_card = settings.HideCard,
            csrfmiddlewaretoken = token
        };
        
        var response = await Request.PostCORSForm(BINGO_URL + "/", token, body);

        if (!response.IsError)
            return response.URL[^22..];

        response.PrintError("Failed to create a new room");
        return null;
    }

    /// <summary>
    /// Joins the room with the given settings
    /// </summary>
    /// <returns>Success of the join</returns>
    public async Task<bool> JoinRoom(JoinRoomSettings settings)
    {
        var body = new
        {
            room = settings.Code,
            password = settings.Password,
            nickname = settings.Nickname,
            is_spectator = settings.IsSpectator
        };
        
        var response = await Request.PostJSON(BINGO_URL + "/api/join-room", body);

        if (response.IsError)
        {
            response.PrintError($"Failed to join room '{settings.Code}'.");
            return false;
        }
        
        var socket = await Request.CreateSocket(
            SOCKETS_URL,
            response.Json()?.Value<string>("socket_key")
        );

        if (socket != null)
            return await Connect(socket);

        Logger.Error("Failed to create the socket.");
        return false;
    }

    /// <summary>
    /// Gets the current board of the room 
    /// </summary>
    public async Task<SquareData[]?> GetBoard()
    {
        if (!IsInRoom)
        {
            Logger.Error("Tried to obtain the board before being connected.");
            return null;
        }

        var url = BINGO_URL + $"/room/{RoomID}/board";

        var response = await Request.Get(url);
        
        if (response.IsError)
            return null;

        var json = response.Parse<JArray>();

        if (json == null)
            return [];
        
        var squares = new SquareData[json.Count];

        var index = 0;
        foreach (var square in json.Children())
        {
            squares[index] = new SquareData(square);
            index++;
        }

        return squares;
    }

    /// <summary>
    /// Changes the team of this client in the room
    /// </summary>
    /// <param name="newTeam">Team to change to</param>
    /// <returns>Succeeded to change the team</returns>
    public async Task<bool> ChangeTeam(Team newTeam)
    {
        if (IsInRoom)
        {
            Logger.Error("Tried to change team before being connected.");
            return false;
        }

        var body = new
        {
            room = RoomID,
            color = newTeam.GetName()
        };
        
        var response = await Request.PutJSON(BINGO_URL + "/api/color", body);

        if (!response.IsError)
            return true;

        response.PrintError($"Failed to change team to '{body.color}'");
        return false;
    }

    private async Task<Response?> SelectSquare(Team team, int index, bool isMarking)
    {
        if (index is <= 0 or > 25)
        {
            Logger.Error("Could not mark the square, as the given index is out of range.");
            return null;
        }
        
        var body = new
        {
            room = RoomID,
            color = team.GetName(),
            slot = index,
            remove_color = !isMarking
        };
        
        return await Request.PutJSON(BINGO_URL + "/api/select", body);
    }
    
    /// <summary>
    /// Marks the square at the given index for a certain team
    /// </summary>
    /// <param name="team">Name of the team</param>
    /// <param name="index">Index of the square</param>
    public async Task<bool> MarkSquare(Team team, int index)
    {
        if (!IsInRoom)
        {
            Logger.Error("Tried to mark a square before being connected.");
            return false;
        }
        
        var response = await SelectSquare(team, index, true);

        if (!response.HasValue)
            return false;

        if (!response.Value.IsError)
            return true;

        response.Value.PrintError($"Failed to mark the square '{index}'");
        return false;

    }
    
    /// <summary>
    /// Clears the square at the given index for a certain team
    /// </summary>
    /// <param name="team">Name of the team</param>
    /// <param name="index">Index of the square</param>
    public async Task<bool> ClearSquare(Team team, int index)
    {
        if (!IsInRoom)
        {
            Logger.Error("Tried to clear a square before being connected.");
            return false;
        }

        var response = await SelectSquare(team, index, false);

        if (!response.HasValue)
            return false;

        if (!response.Value.IsError)
            return true;

        response.Value.PrintError($"Failed to clear the square '{index}'");
        return false;

    }

    /// <summary>
    /// Sends a message in the room
    /// </summary>
    /// <param name="message">Message to send</param>
    public async Task<bool> SendMessage(string message)
    {
        if (!IsInRoom)
        {
            Logger.Error("Tried to send a message before being connected.");
            return false;
        }

        var body = new
        {
            room = RoomID,
            text = message
        };
        
        var response = await Request.PutJSON(BINGO_URL + "/api/chat", body);

        if (!response.IsError)
            return true;

        response.PrintError($"Failed to send the message '{message}'");
        return false;
    }
    
    /// <summary>
    /// Reveals the card for the entire room
    /// </summary>
    public async Task<bool> RevealCard()
    {
        if (!IsInRoom)
        {
            Logger.Error("Tried to reveal the card before being connected.");
            return false;
        }

        var body = new
        {
            room = RoomID
        };

        var response = await Request.PutJSON(BINGO_URL + "/api/revealed", body);

        if (!response.IsError)
            return true;

        response.PrintError("Failed to reveal the card");
        return false;
    }

    /// <summary>
    /// Gets the feed of every event in the room
    /// </summary>
    public async Task<BaseEvent?[]> GetFeed()
    {
        if (!IsInRoom)
        {
            Logger.Error("Tried to get the feed of the room being connected.");
            return [];
        }

        var response = await Request.Get(BINGO_URL + $"/room/{RoomID}/feed");

        if (response.IsError)
        {
            response.PrintError($"Failed to get the feed for the room '{RoomID}'");
            return [];
        }

        var jsonEvents = response.Json()?.GetValue("events");

        if (jsonEvents == null)
            return [];

        var feed = new List<BaseEvent?>();

        foreach (var child in jsonEvents.Children())
        {
            var obj = child?.ToObject<JObject>();
            feed.Add(obj != null ? BaseEvent.ParseEvent(obj) : null);
        }

        return feed.ToArray();
    }

    #endregion

    #region Socket
    
    private ClientWebSocket? _socket;
    private Task? _socketReceiveTask;

    /// <summary>
    /// Connects this client to the servers
    /// </summary>
    /// <returns>Succeeded to connect</returns>
    protected virtual async Task<bool> Connect(ClientWebSocket socket)
    {
        if (IsInRoom)
            return false;

        var isSocketConnected = await socket.HandleTimeout();

        if (!isSocketConnected)
            return false;
        
        _socket = socket;
        _socketReceiveTask = _socket.HandleMessages(OnSocketReceived);

        return await Request.HandleTimeout(() => IsInRoom);
    }

    /// <summary>
    /// Disconnects this client from the servers
    /// </summary>
    /// <returns>Succeeded to disconnect</returns>
    protected virtual async Task<bool> Disconnect()
    {
        if (!IsInRoom || _socket == null)
            return false;

        try
        {
            await _socket.CloseAsync(
                WebSocketCloseStatus.NormalClosure,
                "Client disconnecting",
                CancellationToken.None
            );
            
            if (_socketReceiveTask != null)
                await _socketReceiveTask;
        }
        catch (Exception e)
        {
            Logger.Error(e.Message);
        }

        _socket.Dispose();
        _socket = null;
        _socketReceiveTask = null;
        
        RoomID = null;
        UUID = null;
        return true;
    }
    
    #endregion
    
    #region Events

    private void OnSocketReceived(JObject? json)
    {
        if (json == null)
            return;

        var @event = BaseEvent.ParseEvent(json);
        
        if (@event == null)
            return;

        OnEvent(@event);

        if (IsInRoom || @event is not ConnectedEvent connectedEvent)
            return;

        RoomID = connectedEvent.RoomId;
        UUID = connectedEvent.Player.UUID;
    }

    /// <summary>
    /// Called when this client receives an event
    /// </summary>
    protected abstract void OnEvent(BaseEvent baseEvent);
    
    #endregion
    
    /// <inheritdoc/>
    ~BaseClient() => _ = Disconnect();
}