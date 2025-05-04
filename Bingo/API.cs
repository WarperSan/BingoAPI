using System.Collections.Generic;
using System.Threading.Tasks;
using BingoAPI.Extensions;
using BingoAPI.Helpers;
using BingoAPI.Models;
using BingoAPI.Models.Events;
using BingoAPI.Models.Settings;
using BingoAPI.Network;
using Newtonsoft.Json.Linq;

namespace BingoAPI.Bingo;

public static class API
{
    /// <summary>
    /// Creates a room, joins it and creates a client out of it
    /// </summary>
    public static async Task<T?> CreateRoom<T>(CreateRoomSettings settings) where T : Client, new()
    {
        Logger.Debug("Fetching CORS token...");

        var token = await Request.GetCORSToken(Constants.BINGO_URL);

        if (token == null)
            return null;
        
        Logger.Debug("CORS token fetched!");
        
        Logger.Debug("Creating a new room...");

        var body = new
        {
            room_name = settings.Name,
            passphrase = settings.Password,
            nickname = "LethalBingo",
            game_type = 18, // Custom (Advanced)
            variant_type = settings.VariantType,
            custom_json = settings.Goals.GenerateJSON(),
            lockout_mode = settings.LockoutMode,
            seed = settings.Seed, // Specify seed if needed
            is_spectator = true,
            hide_card = settings.HideCard,
            csrfmiddlewaretoken = token
        };
        
        var response = await Request.PostCORSForm(Constants.CREATE_ROOM_URL, token, body);
        
        // If failed, fetch error
        if (response.IsError)
        {
            response.PrintError("Failed to create a new room");
            return null;
        }
        
        Logger.Debug("Room created!");

        var joinSettings = new JoinRoomSettings
        {
            Code = response.URL[^22..],
            Password = settings.Password,
            Nickname = settings.Nickname,
            IsSpectator = settings.IsSpectator
        };
        
        return await JoinRoom<T>(joinSettings);
    }
    
    /// <summary>
    /// Joins the given room and creates a client out of it
    /// </summary>
    public static async Task<T?> JoinRoom<T>(JoinRoomSettings settings) where T : Client, new()
    {
        Logger.Debug($"Joining the room '{settings.Code}'...");
        
        var body = new
        {
            room = settings.Code,
            password = settings.Password,
            nickname = settings.Nickname,
            is_spectator = settings.IsSpectator
        };
        
        var response = await Request.PostJson(Constants.JOIN_ROOM_URL, body);

        // If failed, fetch error
        if (response.IsError)
        {
            response.PrintError($"Failed to join room '{settings.Code}'.");
            return null;
        }

        var responseJson = response.Json();

        var socket = await Socket.CreateSocket(Constants.SOCKETS_URL, responseJson?.Value<string>("socket_key"));

        if (socket == null)
        {
            Logger.Error("Failed to create the socket.");
            return null;
        }
        
        Logger.Debug("Room joined!");

        var client = new T();

        Logger.Debug("Waiting for connection...");
        var connected = await client.Connect(socket);

        if (!connected)
        {
            Logger.Error("Could not connect before the timeout.");
            return null;
        }
        
        Logger.Debug("Client successfully connected!");

        return client;
    }

    /// <summary>
    /// Fetches the current board of the given room
    /// </summary>
    /// <param name="roomId">ID of the room</param>
    public static async Task<SquareData[]?> GetBoard(string roomId)
    {
        Logger.Debug($"Obtaining the board of the room '{roomId}'...");
        
        var url = string.Format(Constants.GET_BOARD_URL, roomId);

        var response = await Request.Get(url);
        
        if (response.IsError)
        {
            response.PrintError($"Failed to obtain the board of the room '{roomId}'");
            return null;
        }

        Logger.Debug($"Board successfully obtained from the room '{roomId}'!");

        var json = response.Parse<JArray>();

        if (json == null)
            return [];
        
        var squares = new SquareData[json.Count];

        var index = 0;
        foreach (var square in json.Children())
        {
            squares[index] = SquareData.ParseJSON(square);
            index++;
        }

        return squares;
    }

    /// <summary>
    /// Changes the team of the client in the room
    /// </summary>
    /// <param name="roomId">ID of the room</param>
    /// <param name="newTeam">Team to change to</param>
    public static async Task<bool> ChangeTeam(string roomId, Team newTeam)
    {
        Logger.Debug($"Changing team to '{newTeam}'...");
        
        var body = new
        {
            room = roomId,
            color = newTeam.GetName()
        };
        
        var response = await Request.PutJson(Constants.CHANGE_TEAM_URL, body);

        if (response.IsError)
        {
            response.PrintError($"Failed to change team to '{body.color}'");
            return false;
        }

        Logger.Debug($"Team successfully changed to '{newTeam}'!");
        return true;
    }
    
    private static async Task<Response?> SelectSquare(string roomId, Team team, int id, bool isMarking)
    {
        if (id is <= 0 or > Constants.BINGO_SIZE)
        {
            Logger.Error("Could not mark square as the id is out of range.");
            return null;
        }
        
        Logger.Debug($"{(isMarking ? "Marking" : "Clearing")} the square '{id}' for the team '{team}'.");
        
        var body = new
        {
            room = roomId,
            color = team.GetName(),
            slot = id,
            remove_color = !isMarking
        };
        
        return await Request.PutJson(Constants.SELECT_SQUARE_URL, body);
    }

    /// <summary>
    /// Marks a square in the room for a certain team
    /// </summary>
    /// <param name="roomId">ID of the room</param>
    /// <param name="team">Name of the team</param>
    /// <param name="id">Index of the square</param>
    public static async Task<bool> MarkSquare(string roomId, Team team, int id)
    {
        var r = await SelectSquare(roomId, team, id, true);

        if (!r.HasValue)
            return false;

        var response = r.Value;

        if (response.IsError)
        {
            response.PrintError($"Failed to mark the square '{id}'");
            return false;
        }
        
        Logger.Debug($"Square '{id}' successfully marked!");
        return true;
    }
    
    /// <summary>
    /// Clears a square in the room for a certain team
    /// </summary>
    /// <param name="roomId">ID of the room</param>
    /// <param name="team">Name of the team</param>
    /// <param name="id">Index of the square</param>
    public static async Task<bool> ClearSquare(string roomId, Team team, int id)
    {
        var r = await SelectSquare(roomId, team, id, false);

        if (!r.HasValue)
            return false;

        var response = r.Value;

        if (response.IsError)
        {
            response.PrintError($"Failed to clear the square '{id}'");
            return false;
        }
        
        Logger.Debug($"Square '{id}' successfully cleared!");
        return true;
    }
    
    /// <summary>
    /// Sends a message in the room
    /// </summary>
    /// <param name="roomId">ID of the room</param>
    /// <param name="message">Message to send</param>
    public static async Task<bool> SendMessage(string roomId, string message)
    {
        var body = new
        {
            room = roomId,
            text = message
        };
        
        var response = await Request.PutJson(Constants.SEND_MESSAGE_URL, body);

        if (response.IsError)
        {
            response.PrintError($"Failed to send the message '{message}'");
            return false;
        }

        return true;
    }

    /// <summary>
    /// Reveals the card for the entire room
    /// </summary>
    /// <param name="roomId">ID of the room</param>
    public static async Task<bool> RevealCard(string roomId)
    {
        var body = new
        {
            room = roomId
        };

        var response = await Request.PutJson(Constants.REVEAL_CARD_URL, body);

        if (response.IsError)
        {
            response.PrintError("Failed to reveal the card");
            return false;
        }

        return true;
    }

    /// <summary>
    /// Fetches the feed of all the events for the given room
    /// </summary>
    /// <param name="roomId">ID of the room</param>
    public static async Task<Event?[]> GetFeed(string roomId)
    {
        var response = await Request.Get(string.Format(Constants.FEED_URL, roomId));

        if (response.IsError)
        {
            response.PrintError($"Failed to get the feed for the room '{roomId}'");
            return [];
        }

        var jsonEvents = response.Json()?.GetValue("events");

        if (jsonEvents == null)
            return [];

        var feed = new List<Event?>();

        foreach (var child in jsonEvents.Children())
        {
            var obj = child?.ToObject<JObject>();
            feed.Add(obj != null ? Event.ParseEvent(obj) : null);
        }

        return feed.ToArray();
    }
}