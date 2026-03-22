using System.Net.WebSockets;
using System.Text.RegularExpressions;
using BingoAPI.Events;
using BingoAPI.Extensions;
using BingoAPI.Helpers;
using BingoAPI.Models;
using BingoAPI.Settings;
using Newtonsoft.Json.Linq;

namespace BingoAPI.Network;

/// <summary>
/// Handles all HTTP communication with the BingoSync REST API
/// </summary>
internal sealed class BingoSyncApiHandler
{
	/// <summary>
	/// Creates a room with the given settings
	/// </summary>
	/// <returns>
	/// Code of the room
	/// </returns>
	public async Task<string?> CreateRoom(CreateRoomSettings settings)
	{
		const int CUSTOM_GAME_TYPE = 18;
		const int RANDOMIZED_VARIANT_TYPE = 172;
		const int FIXED_BOARD_VARIANT_TYPE = 18;
		const int LOCKOUT_MODE = 2;
		const int NON_LOCKOUT_MODE = 1;

		var tokens = await Request.GetCorsTokens("");

		if (!tokens.HasValue)
		{
			Log.Error("CORS tokens not found.");
			return null;
		}

		var body = new
		{
			room_name = settings.Name,
			passphrase = settings.Password,
			nickname = Plugin.Id,
			game_type = CUSTOM_GAME_TYPE,
			variant_type = settings.IsRandomized ? RANDOMIZED_VARIANT_TYPE : FIXED_BOARD_VARIANT_TYPE,
			custom_json = settings.Goals.GenerateJson(),
			lockout_mode = settings.IsLockout ? LOCKOUT_MODE : NON_LOCKOUT_MODE,
			seed = settings.Seed,
			is_spectator = settings.IsSpectator,
			hide_card = settings.HideCard,
			csrfmiddlewaretoken = tokens.Value._hidden
		};

		var response = await Request.PostCorsForm(
			"/",
			tokens.Value._public,
			tokens.Value._hidden,
			body
		);

		if (response.IsError)
		{
			response.PrintError("Failed to create a new room");
			return null;
		}

		var match = Regex.Match(response.URL, "(?<=/room/)[a-zA-Z\\d-_]+");

		if (!match.Success)
		{
			Log.Error($"Failed to find the room code from the URL '{response.URL}'.");
			return null;
		}

		return match.Value;
	}

	/// <summary>
	/// Joins the room with the given settings
	/// </summary>
	/// <returns>
	///	Socket key of the <see cref="WebSocket"/>
	/// </returns>
	public async Task<string?> JoinRoom(JoinRoomSettings settings)
	{
		var body = new
		{
			room = settings.Code,
			password = settings.Password,
			nickname = settings.Nickname,
			is_spectator = settings.IsSpectator
		};

		var response = await Request.PostJson("/api/join-room", body);

		if (response.IsError)
		{
			response.PrintError($"Failed to join room '{settings.Code}'");
			return null;
		}

		var json = response.ParseJson<JObject>();
		var socketKey = json?.Value<string>("socket_key");

		if (socketKey == null)
			Log.Error($"Expected 'socket_key': {response.Content}");

		return socketKey;
	}

	/// <summary>
	/// Gets the current squares of the given room
	/// </summary>
	public async Task<SquareData[]?> GetSquares(string roomId)
	{
		var response = await Request.Get($"/room/{roomId}/board");

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

		return squares;
	}

	/// <summary>
	/// Changes the team of the client in the room
	/// </summary>
	public async Task<bool> ChangeTeam(string roomId, Team newTeam)
	{
		var body = new
		{
			room = roomId,
			color = newTeam.GetName()
		};

		var response = await Request.PutJson("/api/color", body);

		if (response.IsError)
		{
			response.PrintError($"Failed to change team to '{body.color}'");
			return false;
		}

		return true;
	}

	/// <summary>
	/// Marks the square at the given index for a certain team
	/// </summary>
	public async Task<bool> MarkSquare(string roomId, Team team, int index)
	{
		var body = new
		{
			room = roomId,
			color = team.GetName(),
			slot = index,
			remove_color = false
		};

		var response = await Request.PutJson("/api/select", body);

		if (response.IsError)
		{
			response.PrintError($"Failed to mark the square '{index}'");
			return false;
		}

		return true;
	}

	/// <summary>
	/// Clears the square at the given index for a certain team
	/// </summary>
	public async Task<bool> ClearSquare(string roomId, Team team, int index)
	{
		var body = new
		{
			room = roomId,
			color = team.GetName(),
			slot = index,
			remove_color = true
		};

		var response = await Request.PutJson("/api/select", body);

		if (response.IsError)
		{
			response.PrintError($"Failed to clear the square '{index}'");
			return false;
		}

		return true;
	}

	/// <summary>
	/// Sends a message in the room
	/// </summary>
	public async Task<bool> SendMessage(string roomId, string message)
	{
		var body = new
		{
			room = roomId,
			text = message
		};

		var response = await Request.PutJson("/api/chat", body);

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
	public async Task<bool> RevealCard(string roomId)
	{
		var body = new
		{
			room = roomId
		};

		var response = await Request.PutJson("/api/revealed", body);

		if (response.IsError)
		{
			response.PrintError("Failed to reveal the card");
			return false;
		}

		return true;
	}

	/// <summary>
	/// Gets the feed of every event in the room
	/// </summary>
	public async Task<BaseEvent[]?> GetFeed(string roomId)
	{
		var response = await Request.Get($"/room/{roomId}/feed");

		if (response.IsError)
		{
			response.PrintError($"Failed to get the feed for the room '{roomId}'");
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

		return feed.ToArray();
	}
}
