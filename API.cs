﻿using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using BingoAPI.Data;
using BingoAPI.Events;
using BingoAPI.Extensions;
using BingoAPI.Helpers;
using BingoAPI.Network;
using Newtonsoft.Json.Linq;

namespace BingoAPI;

public static class API
{
    // Size
    public const int BINGO_HEIGHT = 5;
    public const int BINGO_WIDTH = 5;
    public const int MAX_BINGO_SIZE = BINGO_HEIGHT * BINGO_WIDTH;
    
    // URLs
    public const string SOCKETS_URL = "wss://sockets.bingosync.com/broadcast";
    private const string BINGO_URL = "https://bingosync.com";
    private const string CREATE_ROOM_URL = BINGO_URL + "/";
    
    private const string GET_BOARD_URL = BINGO_URL + "/room/{0}/board";
    private const string FEED_URL = BINGO_URL + "/room/{0}/feed";
    
    private const string JOIN_ROOM_URL = BINGO_URL + "/api/join-room";
    private const string CHANGE_TEAM_URL = BINGO_URL + "/api/color";
    private const string SELECT_SQUARE_URL = BINGO_URL + "/api/select";
    private const string SEND_MESSAGE_URL = BINGO_URL + "/api/chat";
    private const string REVEAL_CARD_URL = BINGO_URL + "/api/revealed";
    private const string NEW_CARD_URL = BINGO_URL + "/api/new-card";
    
    /// <summary>
    /// Creates a room, joins it and creates a client out of it
    /// </summary>
    /// <param name="roomName">Name of the room</param>
    /// <param name="roomPassword">Password of the room</param>
    /// <param name="nickName">Nickname of the user</param>
    /// <param name="isRandomized">Is the board randomized or not</param>
    /// <param name="boardJSON">List of goals</param>
    /// <param name="isLockout">Is the board in lockout or not</param>
    /// <param name="seed">Seed to use for the board</param>
    /// <param name="isSpectator">Is the user a spectator or not</param>
    /// <param name="hideCard">Is the card hidden initially or not</param>
    public static async Task<T?> CreateRoom<T>(
        string roomName,
        string roomPassword,
        string nickName,
        bool isRandomized,
        string boardJSON,
        bool isLockout,
        string seed,
        bool isSpectator,
        bool hideCard
    ) where T : BingoClient {
        
        Logger.Debug("Fetching CORS token...");

        var token = await Request.GetCORSToken(BINGO_URL);

        if (token == null)
            return null;
        
        Logger.Debug("CORS token fetched!");
        
        Logger.Debug("Creating a new room...");

        var body = new
        {
            room_name = roomName,
            passphrase = roomPassword,
            nickname = "LethalBingo",
            game_type = 18, // Custom (Advanced)
            variant_type = isRandomized ? 172 : 18, // 18 = Fixed Board, 172 = Randomized 
            custom_json = boardJSON,
            lockout_mode = isLockout ? 2 : 1, // 1 = Non-Lockout, 2 = Lockout
            seed = seed, // Specify seed if needed
            is_spectator = true,
            hide_card = hideCard,
            csrfmiddlewaretoken = token
        };
        
        var response = await Request.PostCORSForm(CREATE_ROOM_URL, token, body);
        
        // If failed, fetch error
        if (response.IsError)
        {
            response.PrintError("Failed to create a new room");
            return null;
        }
        
        Logger.Debug("Room created!");
        var roomCode = response.URL[^22..];
        
        return await JoinRoom<T>(roomCode, roomPassword, nickName, isSpectator, true);
    }
    
    /// <summary>
    /// Joins the given room and creates a client out of it
    /// </summary>
    /// <param name="roomId">ID of the room</param>
    /// <param name="roomPassword">Password of the room</param>
    /// <param name="nickName">Nickname of the user</param>
    /// <param name="isSpectator">Is the user a spectator or not</param>
    /// <param name="isCreator">Is the creator of the room or not</param>
    public static async Task<T?> JoinRoom<T>(
        string roomId, 
        string roomPassword, 
        string nickName, 
        bool isSpectator, 
        bool isCreator
    ) where T : BingoClient {
        Logger.Debug($"Joining the room '{roomId}'...");
        
        var body = new
        {
            room = roomId,
            password = roomPassword,
            nickname = nickName,
            is_spectator = isSpectator
        };
        
        var response = await Request.PostJson(JOIN_ROOM_URL, body);

        // If failed, fetch error
        if (response.IsError)
        {
            response.PrintError($"Failed to join room '{roomId}'.");
            return null;
        }

        var responseJson = response.Json();

        var socket = await Socket.CreateSocket(SOCKETS_URL, responseJson?.Value<string>("socket_key"));

        if (socket == null)
        {
            Logger.Error("Failed to create the socket.");
            return null;
        }
        
        Logger.Debug("Room joined!");

        T client;

        try
        {
            client = (T)Activator.CreateInstance(typeof(T), socket, isCreator);
        }
        catch (Exception e)
        {
            Logger.Error(e.Message);
            return null;
        }

        Logger.Debug("Waiting for connection...");
        var connected = await client.WaitForConnection(60_000);

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
        
        var url = string.Format(GET_BOARD_URL, roomId);

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
    public static async Task<bool> ChangeTeam(string roomId, BingoTeam newTeam)
    {
        Logger.Debug($"Changing team to '{newTeam}'...");
        
        var body = new
        {
            room = roomId,
            color = newTeam.GetName()
        };
        
        var response = await Request.PutJson(CHANGE_TEAM_URL, body);

        if (response.IsError)
        {
            response.PrintError($"Failed to change team to '{body.color}'");
            return false;
        }

        Logger.Debug($"Team successfully changed to '{newTeam}'!");
        return true;
    }
    
    private static async Task<Response?> SelectSquare(string roomId, BingoTeam team, int id, bool isMarking)
    {
        if (id is <= 0 or > MAX_BINGO_SIZE)
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
        
        return await Request.PutJson(SELECT_SQUARE_URL, body);
    }

    /// <summary>
    /// Marks a square in the room for a certain team
    /// </summary>
    /// <param name="roomId">ID of the room</param>
    /// <param name="team">Name of the team</param>
    /// <param name="id">Index of the square</param>
    public static async Task<bool> MarkSquare(string roomId, BingoTeam team, int id)
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
    public static async Task<bool> ClearSquare(string roomId, BingoTeam team, int id)
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
        
        var response = await Request.PutJson(SEND_MESSAGE_URL, body);

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

        var response = await Request.PutJson(REVEAL_CARD_URL, body);

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
        var response = await Request.Get(string.Format(FEED_URL, roomId));

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