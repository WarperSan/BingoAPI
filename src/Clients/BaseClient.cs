using System.Text.RegularExpressions;
using BingoAPI.Events;
using BingoAPI.Extensions;
using BingoAPI.Helpers;
using BingoAPI.Models;
using BingoAPI.Settings;
using BingoAPI.Network;
using Newtonsoft.Json.Linq;

namespace BingoAPI.Clients;

/// <summary>
/// Class that represents the bare minimum for a client
/// </summary>
public abstract class BaseClient : IDisposable
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
	// ReSharper disable once InconsistentNaming
	public string? UUID { get; protected set; }

	protected BaseClient()
	{
		_eventDispatcher = new EventDispatcher(OnEventReceived);
		_socketHandler = new SocketHandler();
		_apiHandler = new BingoSyncApiHandler();
	}

	#region API

	private readonly BingoSyncApiHandler _apiHandler;

	/// <summary>
	/// Creates a room with the given settings
	/// </summary>
	/// <returns>Code of the room or null if the room couldn't be created</returns>
	public async Task<bool> CreateRoom(CreateRoomSettings settings)
	{
		Log.Info($"Creating the room '{settings.Name}'...");

		var code = await _apiHandler.CreateRoom(settings);

		if (code == null)
			return false;

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

		var socketKey = await _apiHandler.JoinRoom(settings);

		if (socketKey == null)
			return false;

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
		if (RoomID == null)
		{
			Log.Error("Tried to obtain the squares before being connected.");
			return null;
		}

		Log.Info($"Fetching the squares of the room '{RoomID}'...");

		var squares = await _apiHandler.GetSquares(RoomID);

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
		if (RoomID == null)
		{
			Log.Error("Tried to change team before being connected.");
			return false;
		}

		Log.Info($"Changing the team of the player '{UUID}' to '{newTeam}'...");

		var success = await _apiHandler.ChangeTeam(RoomID, newTeam);

		if (success)
			Log.Info($"Changed the team of the player '{UUID}'.");

		return success;
	}

	/// <summary>
	/// Marks the square at the given index for a certain team
	/// </summary>
	/// <param name="team">Name of the team</param>
	/// <param name="index">Index of the square</param>
	/// <returns>Succeeded to mark the square</returns>
	public async Task<bool> MarkSquare(Team team, int index)
	{
		if (RoomID == null)
		{
			Log.Error("Tried to mark a square before being connected.");
			return false;
		}

		Log.Info($"Marking the square #{index} for the team '{team}'...");

		var success = await _apiHandler.MarkSquare(RoomID, team, index);

		if (success)
			Log.Info($"Marked the square #{index} for the team '{team}'.");

		return success;
	}

	/// <summary>
	/// Clears the square at the given index for a certain team
	/// </summary>
	/// <param name="team">Name of the team</param>
	/// <param name="index">Index of the square</param>
	/// <returns>Succeeded to clear the square</returns>
	public async Task<bool> ClearSquare(Team team, int index)
	{
		if (RoomID == null)
		{
			Log.Error("Tried to clear a square before being connected.");
			return false;
		}

		Log.Info($"Clearing the square #{index} for the team '{team}'...");

		var success = await _apiHandler.ClearSquare(RoomID, team, index);

		if (success)
			Log.Info($"Cleared the square #{index} for the team '{team}'.");

		return success;
	}

	/// <summary>
	/// Sends a message in the room
	/// </summary>
	/// <param name="message">Message to send</param>
	/// <returns>Succeeded to send the message</returns>
	public async Task<bool> SendMessage(string message)
	{
		if (RoomID == null)
		{
			Log.Error("Tried to send a message before being connected.");
			return false;
		}

		Log.Info($"Sending the following chat message as the player '{UUID}': '{message}'...");

		var success = await _apiHandler.SendMessage(RoomID, message);

		if (success)
			Log.Info($"Sent the following chat message as the player '{UUID}': '{message}'.");

		return success;
	}

	/// <summary>
	/// Reveals the card for the entire room
	/// </summary>
	/// <returns>Succeeded to reveal the card</returns>
	public async Task<bool> RevealCard()
	{
		if (RoomID == null)
		{
			Log.Error("Tried to reveal the card before being connected.");
			return false;
		}

		Log.Info($"Revealing the card in the room '{RoomID}' as the player '{UUID}'...");

		var success = await _apiHandler.RevealCard(RoomID);

		if (success)
			Log.Info($"Revealed the card in the room '{RoomID}' as the player '{UUID}'.");

		return success;
	}

	/// <summary>
	/// Gets the feed of every event in the room
	/// </summary>
	/// <returns>Succeeded to get the feed</returns>
	public async Task<BaseEvent[]?> GetFeed()
	{
		if (RoomID == null)
		{
			Log.Error("Tried to get the feed of the room being connected.");
			return null;
		}

		Log.Info($"Fetching the feed of the room '{RoomID}'...");

		var events = await _apiHandler.GetFeed(RoomID);

		if (events == null)
			return [];

		Log.Info($"Fetched the feed of the room '{RoomID}'.");
		return events;
	}

	#endregion

	#region Socket

	private readonly SocketHandler _socketHandler;

	/// <summary>
	/// Connects this client to the servers
	/// </summary>
	/// <returns>Succeeded to connect</returns>
	protected virtual async Task<bool> Connect(string socketKey)
	{
		if (IsInRoom)
			return false;

		Log.Info("Connecting to the server...");

		var success = await _socketHandler.Connect(socketKey, _eventDispatcher.OnMessageReceived);

		if (success)
			Log.Info("Connected to the server.");
		else
			Log.Error("Failed to create the socket.");

		return success;
	}

	/// <summary>
	/// Disconnects this client from the servers
	/// </summary>
	/// <returns>Succeeded to disconnect</returns>
	protected virtual async Task<bool> Disconnect()
	{
		if (!IsInRoom)
			return false;

		Log.Info("Disconnecting from the server...");

		await _socketHandler.Disconnect();

		Log.Info("Disconnected from the server.");

		RoomID = null;
		UUID = null;

		Log.Info("Disconnected from the server.");
		return true;
	}

	#endregion

	#region Events

	private readonly EventDispatcher _eventDispatcher;

	private void OnEventReceived(BaseEvent baseEvent)
	{
		if (!IsInRoom && baseEvent is ConnectedEvent connectedEvent)
		{
			RoomID = connectedEvent.RoomId;
			UUID = connectedEvent.Player.UUID;
		}

		OnEvent(baseEvent);
	}

	/// <summary>
	/// Called when this client receives an event
	/// </summary>
	protected abstract void OnEvent(BaseEvent baseEvent);

	#endregion

	/// <inheritdoc />
	public void Dispose()
	{
		_socketHandler.Dispose();
	}
}
