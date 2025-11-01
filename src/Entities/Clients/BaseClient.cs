using System;
using System.Collections.Generic;
using System.Net.WebSockets;
using System.Text.RegularExpressions;
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
    //public const string NEW_CARD_URL = "/api/new-card";

    /// <summary>
    /// Current room ID of this client
    /// </summary>
    public string? RoomID { get; protected set; }

    /// <summary>
    /// Checks if this client is in a room
    /// </summary>
    public bool IsInRoom => RoomID != null;

    /// <summary>
    /// Current UUID of this client
    /// </summary>
    public string? UUID { get; protected set; }

    #region API

    /// <summary>
    /// Creates a room with the given settings
    /// </summary>
    /// <returns>Code of the room or null if the room couldn't be created</returns>
    public async Task<bool> CreateRoom(CreateRoomSettings settings)
    {
        const int CUSTOM_GAME_TYPE = 18;
        const int RANDOMIZED_VARIANT_TYPE = 172;
        const int FIXED_BOARD_VARIANT_TYPE = 18;
        const int LOCKOUT_MODE = 2;
        const int NON_LOCKOUT_MODE = 1;

        Log.Info($"Creating the room '{settings.Name}'...");

        var tokens = await Request.GetCORSTokens("");

        if (!tokens.HasValue)
        {
            Log.Error("CORS tokens not found.");
            return false;
        }

        var body = new
        {
            room_name = settings.Name,
            passphrase = settings.Password,
            nickname = MyPluginInfo.PLUGIN_GUID,
            game_type = CUSTOM_GAME_TYPE,
            variant_type = settings.IsRandomized ? RANDOMIZED_VARIANT_TYPE : FIXED_BOARD_VARIANT_TYPE,
            custom_json = settings.Goals.GenerateJSON(),
            lockout_mode = settings.IsLockout ? 2 : 1, // 2 = Lockout, 1 = Non-Lockout 
            seed = settings.Seed,
            is_spectator = settings.IsSpectator,
            hide_card = settings.HideCard,
            csrfmiddlewaretoken = tokens.Value._hidden
        };

        var response = await Request.PostCORSForm(
            "/",
            tokens.Value._public,
            tokens.Value._hidden,
            body
        );

        if (response.IsError)
        {
            response.PrintError("Failed to create a new room");
            return false;
        }

        var match = Regex.Match(response.URL, "(?<=/room/)[a-zA-Z\\d-_]+");

        if (!match.Success)
        {
            Log.Error($"Failed to find the room code from the URL '{response.URL}'.");
            return false;
        }

        var code = match.Value;

        Log.Info($"Room '{settings.Name}' was created with the code '{code}'.");

        return await JoinRoom(new JoinRoomSettings
        {
            Code = code,
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
        Log.Info($"Joining room '{settings.Code}'...");

        var body = new
        {
            room = settings.Code,
            password = settings.Password,
            nickname = settings.Nickname,
            is_spectator = settings.IsSpectator
        };

        var response = await Request.PostJSON("/api/join-room", body);

        if (response.IsError)
        {
            response.PrintError($"Failed to join room '{settings.Code}'");
            return false;
        }

        var json = response.ParseJson<JObject>();
        var socketKey = json?.Value<string>("socket_key");

        if (socketKey == null)
        {
            Log.Error($"Expected 'socket_key': {response.Content}");
            return false;
        }

        Log.Info($"Room '{settings.Code}' was joined.");
        return await Connect(socketKey);
    }

    /// <summary>
    /// Leaves the room
    /// </summary>
    /// <returns>Succeeded to leave the room</returns>
    public async Task<bool> LeaveRoom()
    {
        if (!IsInRoom)
        {
            Log.Error("Tried to leave the room before being connected.");
            return false;
        }

        var roomId = RoomID;

        Log.Info($"Leaving the room '{roomId}'...");

        var hasDisconnected = await Disconnect();

        if (!hasDisconnected)
        {
            Log.Error($"Failed to disconnected from the room '{roomId}'.");
            return false;
        }

        Log.Info($"Left the room '{roomId}'.");
        return true;
    }

    /// <summary>
    /// Gets the current squares of the room 
    /// </summary>
    /// <returns>Squares fetched or null if not found</returns>
    public async Task<SquareData[]?> GetSquares()
    {
        if (!IsInRoom)
        {
            Log.Error("Tried to obtain the squares before being connected.");
            return null;
        }

        Log.Info($"Fetching the squares of the room '{RoomID}'...");

        var response = await Request.Get($"/room/{RoomID}/board");

        if (response.IsError)
        {
            response.PrintError("Failed to obtain the squares");
            return null;
        }

        var json = response.ParseJson<JArray>();

        if (json == null)
        {
            Log.Error($"Expected an array of squares: {response.Content}");
            return null;
        }

        var squares = new SquareData[json.Count];
        var index = 0;

        foreach (var square in json.Children())
        {
            squares[index] = new SquareData(square);
            index++;
        }

        Log.Info($"Squares of the room '{RoomID}' was fetched.");
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
            Log.Error("Tried to change team before being connected.");
            return false;
        }

        Log.Info($"Changing the team of the player '{UUID}' to '{newTeam}'...");

        var body = new
        {
            room = RoomID,
            color = newTeam.GetName()
        };

        var response = await Request.PutJSON("/api/color", body);

        if (response.IsError)
        {
            response.PrintError($"Failed to change team to '{body.color}'");
            return false;
        }

        Log.Info($"Changed the team of the player '{UUID}'.");
        return true;
    }

    /// <summary>
    /// Marks the square at the given index for a certain team
    /// </summary>
    /// <param name="team">Name of the team</param>
    /// <param name="index">Index of the square</param>
    /// <returns>Succeeded to mark the square</returns>
    public async Task<bool> MarkSquare(Team team, int index)
    {
        if (!IsInRoom)
        {
            Log.Error("Tried to mark a square before being connected.");
            return false;
        }

        Log.Info($"Marking the square #{index} for the team '{team}'...");

        var body = new
        {
            room = RoomID,
            color = team.GetName(),
            slot = index,
            remove_color = false
        };

        var response = await Request.PutJSON("/api/select", body);

        if (response.IsError)
        {
            response.PrintError($"Failed to mark the square '{index}'");
            return false;
        }

        Log.Info($"Marked the square #{index} for the team '{team}'.");
        return true;
    }

    /// <summary>
    /// Clears the square at the given index for a certain team
    /// </summary>
    /// <param name="team">Name of the team</param>
    /// <param name="index">Index of the square</param>
    /// <returns>Succeeded to clear the square</returns>
    public async Task<bool> ClearSquare(Team team, int index)
    {
        if (!IsInRoom)
        {
            Log.Error("Tried to clear a square before being connected.");
            return false;
        }

        Log.Info($"Clearing the square #{index} for the team '{team}'...");

        var body = new
        {
            room = RoomID,
            color = team.GetName(),
            slot = index,
            remove_color = true
        };

        var response = await Request.PutJSON("/api/select", body);

        if (response.IsError)
        {
            response.PrintError($"Failed to clear the square '{index}'");
            return false;
        }

        Log.Info($"Cleared the square #{index} for the team '{team}'.");
        return true;
    }

    /// <summary>
    /// Sends a message in the room
    /// </summary>
    /// <param name="message">Message to send</param>
    /// <returns>Succeeded to send the message</returns>
    public async Task<bool> SendMessage(string message)
    {
        if (!IsInRoom)
        {
            Log.Error("Tried to send a message before being connected.");
            return false;
        }

        Log.Info($"Sending the following chat message as the player '{UUID}': '{message}'...");

        var body = new
        {
            room = RoomID,
            text = message
        };

        var response = await Request.PutJSON("/api/chat", body);

        if (response.IsError)
        {
            response.PrintError($"Failed to send the message '{message}'");
            return false;
        }

        Log.Info($"Sent the following chat message as the player '{UUID}': '{message}'.");
        return true;
    }

    /// <summary>
    /// Reveals the card for the entire room
    /// </summary>
    /// <returns>Succeeded to reveal the card</returns>
    public async Task<bool> RevealCard()
    {
        if (!IsInRoom)
        {
            Log.Error("Tried to reveal the card before being connected.");
            return false;
        }

        Log.Info($"Revealing the card in the room '{RoomID}' as the player '{UUID}'...");

        var body = new
        {
            room = RoomID
        };

        var response = await Request.PutJSON("/api/revealed", body);

        if (response.IsError)
        {
            response.PrintError("Failed to reveal the card");
            return false;
        }

        Log.Info($"Revealed the card in the room '{RoomID}' as the player '{UUID}'.");
        return true;
    }

    /// <summary>
    /// Gets the feed of every event in the room
    /// </summary>
    /// <returns>Succeeded to get the feed</returns>
    public async Task<BaseEvent[]?> GetFeed()
    {
        if (!IsInRoom)
        {
            Log.Error("Tried to get the feed of the room being connected.");
            return null;
        }

        Log.Info($"Fetching the feed of the room '{RoomID}'...");

        var response = await Request.Get($"/room/{RoomID}/feed");

        if (response.IsError)
        {
            response.PrintError($"Failed to get the feed for the room '{RoomID}'");
            return null;
        }

        var json = response.ParseJson<JObject>();
        var jsonEvents = json?.GetValue("events");

        if (jsonEvents == null)
        {
            Log.Error($"Expected an array of events: {response.Content}");
            return null;
        }

        var feed = new List<BaseEvent>();

        foreach (var child in jsonEvents.Children())
        {
            var obj = child?.ToObject<JObject>();

            if (obj == null)
                continue;

            var @event = BaseEvent.ParseEvent(obj);

            if (@event == null)
                continue;

            feed.Add(@event);
        }

        Log.Info($"Fetched the feed of the room '{RoomID}'.");
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
    protected virtual async Task<bool> Connect(string socketKey)
    {
        if (IsInRoom)
            return false;

        Log.Info("Connecting to the server...");

        var socket = await Request.CreateSocket(
            "wss://sockets.bingosync.com/broadcast",
            socketKey
        );

        if (socket == null)
        {
            Log.Error("Failed to create the socket.");
            return false;
        }

        var cts = new CancellationTokenSource();

        _socket = socket;
        _socketReceiveTask = _socket.HandleMessages(OnSocketReceived, cts.Token);
        _ctSource = cts;

        float timer = 30_000;

        while (!IsInRoom && timer > 0)
        {
            await Task.Delay(25, cts.Token);
            timer -= 25;
        }

        if (timer <= 0)
        {
            Log.Error("Waiting for handshake has timed out.");
            return false;
        }

        Log.Info("Connected to the server.");
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

        Log.Info("Disconnecting from the server...");

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
            catch (OperationCanceledException)
            { /* Expected */
            }
            catch (ObjectDisposedException)
            { /* Expected */
            }
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

        Log.Info("Disconnected from the server.");
        return true;
    }

    #endregion

    #region Events

    private void OnSocketReceived(string json)
    {
        Log.Debug($"Event received: {json}");

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