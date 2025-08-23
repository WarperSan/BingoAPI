using System;
using System.Collections.Generic;
using System.Net.WebSockets;
using System.Threading;
using System.Threading.Tasks;
using BingoAPI.Entities.Events;
using BingoAPI.Extensions;
using BingoAPI.Helpers;
using BingoAPI.Models;
using BingoAPI.Models.Settings;
using BingoAPI.Network;
using Newtonsoft.Json.Linq;

namespace BingoAPI.Entities.Clients;

/// <summary>
/// Class that represents the bare minimum for a client
/// </summary>
public abstract class BaseClient : IAsyncDisposable
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
    public async Task<bool> CreateRoom(CreateRoomSettings settings)
    {
        Log.Debug("Creating room...");

        var token = await Request.GetCORSToken(BINGO_URL);

        if (token == null)
        {
            Log.Error("CORS Token not found.");
            return false;
        }

        var body = new
        {
            room_name = settings.Name,
            passphrase = settings.Password,
            nickname = MyPluginInfo.PLUGIN_GUID,
            game_type = 18, // Custom (Advanced)
            variant_type = settings.IsRandomized ? 172 : 18, // 172 = Randomized, 18 = Fixed Board
            custom_json = settings.Goals.GenerateJSON(),
            lockout_mode = settings.IsLockout ? 2 : 1, // 1 = Non-Lockout, 2 = Lockout
            seed = settings.Seed,
            is_spectator = settings.IsSpectator,
            hide_card = settings.HideCard,
            csrfmiddlewaretoken = token
        };
        
        var response = await Request.PostCORSForm(BINGO_URL + "/", token, body);

        if (response.IsError)
        {
            response.PrintError("Failed to create a new room");
            return false;
        }
        
        Log.Debug("Room created.");
        
        return await JoinRoom(new JoinRoomSettings
        {
            Code = response.URL[^22..],
            Nickname = settings.Nickname,
            Password = settings.Password,
            IsSpectator = settings.IsSpectator
        });
    }

    /// <summary>
    /// Joins the room with the given settings
    /// </summary>
    /// <returns>Succeeded to join the room</returns>
    public async Task<bool> JoinRoom(JoinRoomSettings settings)
    {
        Log.Debug($"Joining room '{settings.Code}'...");

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
            response.PrintError($"Failed to join room '{settings.Code}'");
            return false;
        }
        
        var socket = await Request.CreateSocket(
            SOCKETS_URL,
            response.Json()?.Value<string>("socket_key")
        );

        if (socket == null)
        {
            Log.Error("Failed to create the socket.");
            return false;
        }

        Log.Debug("Joined the room.");
        return await Connect(socket);
    }

    /// <summary>
    /// Gets the current board of the room 
    /// </summary>
    public async Task<SquareData[]?> GetBoard()
    {
        Log.Debug("Getting board...");
        
        if (!IsInRoom)
        {
            Log.Error("Tried to obtain the board before being connected.");
            return null;
        }

        var response = await Request.Get(BINGO_URL + $"/room/{RoomID}/board");

        if (response.IsError)
        {
            response.PrintError("Failed to obtain the board");
            return null;
        }

        var json = response.Parse<JArray>();
        var count = json?.Count ?? 0;

        var squares = new SquareData[count];

        if (json != null)
        {
            var index = 0;
            foreach (var square in json.Children())
            {
                squares[index] = new SquareData(square);
                index++;
            }
        }

        Log.Debug($"Board has '{count}' squares.");
        return squares;
    }

    /// <summary>
    /// Changes the team of this client in the room
    /// </summary>
    /// <param name="newTeam">Team to change to</param>
    /// <returns>Succeeded to change the team</returns>
    public async Task<bool> ChangeTeam(Team newTeam)
    {
        Log.Debug($"Changing team to '{newTeam}'...");

        if (IsInRoom)
        {
            Log.Error("Tried to change team before being connected.");
            return false;
        }

        var body = new
        {
            room = RoomID,
            color = newTeam.GetName()
        };
        
        var response = await Request.PutJSON(BINGO_URL + "/api/color", body);

        if (response.IsError)
        {
            response.PrintError($"Failed to change team to '{body.color}'");
            return false;
        }

        Log.Debug("Changed team.");
        return true;
    }

    private async Task<Response?> SelectSquare(Team team, int index, bool isMarking)
    {
        if (index is <= 0 or > 25)
        {
            Log.Error("Could not mark the square, as the given index is out of range.");
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
        Log.Debug($"Marking the square '{index}' for team '{team}'...");
        
        if (!IsInRoom)
        {
            Log.Error("Tried to mark a square before being connected.");
            return false;
        }
        
        var response = await SelectSquare(team, index, true);

        if (!response.HasValue)
            return false;

        if (response.Value.IsError)
        {
            response.Value.PrintError($"Failed to mark the square '{index}'");
            return false;
        }

        Log.Debug("Marked the square.");
        return true;
    }
    
    /// <summary>
    /// Clears the square at the given index for a certain team
    /// </summary>
    /// <param name="team">Name of the team</param>
    /// <param name="index">Index of the square</param>
    public async Task<bool> ClearSquare(Team team, int index)
    {
        Log.Debug($"Clearing the square '{index}' from team '{team}'...");
        
        if (!IsInRoom)
        {
            Log.Error("Tried to clear a square before being connected.");
            return false;
        }

        var response = await SelectSquare(team, index, false);

        if (!response.HasValue)
            return false;

        if (response.Value.IsError)
        {
            response.Value.PrintError($"Failed to clear the square '{index}'");
            return false;
        }
        
        Log.Debug("Cleared the square.");
        return true;
    }

    /// <summary>
    /// Sends a message in the room
    /// </summary>
    /// <param name="message">Message to send</param>
    public async Task<bool> SendMessage(string message)
    {
        Log.Debug($"Sending message '{message}'...");

        if (!IsInRoom)
        {
            Log.Error("Tried to send a message before being connected.");
            return false;
        }

        var body = new
        {
            room = RoomID,
            text = message
        };
        
        var response = await Request.PutJSON(BINGO_URL + "/api/chat", body);

        if (response.IsError)
        {
            response.PrintError($"Failed to send the message '{message}'");
            return false;
        }
        
        Log.Debug("Message sent.");
        return true;
    }
    
    /// <summary>
    /// Reveals the card for the entire room
    /// </summary>
    public async Task<bool> RevealCard()
    {
        Log.Debug("Revealing the card...");

        if (!IsInRoom)
        {
            Log.Error("Tried to reveal the card before being connected.");
            return false;
        }

        var body = new
        {
            room = RoomID
        };

        var response = await Request.PutJSON(BINGO_URL + "/api/revealed", body);

        if (response.IsError)
        {
            response.PrintError("Failed to reveal the card");
            return false;
        }

        Log.Debug("Card revealed.");
        return true;
    }

    /// <summary>
    /// Gets the feed of every event in the room
    /// </summary>
    public async Task<BaseEvent?[]> GetFeed()
    {
        Log.Debug("Getting feed...");

        if (!IsInRoom)
        {
            Log.Error("Tried to get the feed of the room being connected.");
            return [];
        }

        var response = await Request.Get(BINGO_URL + $"/room/{RoomID}/feed");

        if (response.IsError)
        {
            response.PrintError($"Failed to get the feed for the room '{RoomID}'");
            return [];
        }

        var jsonEvents = response.Json()?.GetValue("events");
        var feed = new List<BaseEvent?>();

        if (jsonEvents != null)
        {
            foreach (var child in jsonEvents.Children())
            {
                var obj = child?.ToObject<JObject>();
                feed.Add(obj != null ? BaseEvent.ParseEvent(obj) : null);
            }
        }

        Log.Debug($"Feed has {feed.Count} events.");
        return feed.ToArray();
    }

    #endregion

    #region Socket
    
    private ClientWebSocket? _socket;
    private Task? _socketReceiveTask;
    private CancellationTokenSource? _ctSource;

    /// <summary>
    /// Connects this client to the servers
    /// </summary>
    /// <returns>Succeeded to connect</returns>
    protected virtual async Task<bool> Connect(ClientWebSocket socket)
    {
        if (IsInRoom)
            return false;
        
        Log.Debug("Connecting...");

        var isSocketConnected = await socket.HandleTimeout();

        if (!isSocketConnected)
        {
            Log.Debug("Connecting to the socket has timed out.");
            return false;
        }
        
        var cts = new CancellationTokenSource();
        
        _socket = socket;
        _socketReceiveTask = _socket.HandleMessages(OnSocketReceived, cts.Token);
        _ctSource = cts;

        var hasTimeout = await Request.HandleTimeout(() => IsInRoom);

        if (!hasTimeout)
        {
            Log.Debug("Waiting for handshake has timed out.");
            return false;
        }

        Log.Debug("Connected.");
        return true;
    }

    /// <summary>
    /// Disconnects this client from the servers
    /// </summary>
    /// <returns>Succeeded to disconnect</returns>
    protected virtual async Task<bool> Disconnect()
    {
        if (!IsInRoom || _socket == null)
            return false;
        
        Log.Debug("Disconnecting...");

        try
        {
            if (_socket.State == WebSocketState.Open)
            {
                await _socket.CloseAsync(
                    WebSocketCloseStatus.NormalClosure,
                    "Client disconnecting",
                    CancellationToken.None
                );
            }
        }
        catch (Exception e)
        {
            Log.Error($"Error closing WebSocket: {e.Message}");
        }

        _ctSource?.Cancel();

        if (_socketReceiveTask != null)
        {
            try
            {
                await _socketReceiveTask;
            }
            catch (OperationCanceledException) { /* Expected */ }
            catch (ObjectDisposedException) { /* Expected */ }
            catch (Exception ex)
            {
                Log.Error($"Error in receive task during disconnect: {ex.Message}");
            }
        }

        _socket.Dispose();
        _socket = null;

        _ctSource?.Dispose();
        _ctSource = null;

        _socketReceiveTask = null;

        RoomID = null;
        UUID = null;

        Log.Debug("Disconnected.");
        
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

    #region IAsyncDisposable

    /// <inheritdoc/>
    public async ValueTask DisposeAsync() => await Disconnect();

    #endregion
}