using BingoAPI.Events;
using BingoAPI.Helpers;
using BingoAPI.Models;
using BingoAPI.Settings;
using BingoAPI.Network;
using System.Diagnostics.CodeAnalysis;

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
	public string? RoomId { get; private set; }

	/// <summary>
	/// Checks if this client is in a room
	/// </summary>
	[MemberNotNullWhen(true, nameof(RoomId))]
	public bool IsInRoom => RoomId != null;

	/// <summary>
	/// Current UUID of this client
	/// </summary>
	public string? UUID { get; private set; }

	protected BaseClient()
	{
		_eventDispatcher = new EventDispatcher(OnEventReceived);
		_socketHandler = new SocketHandler();
		_apiHandler = new BingoSyncApiHandler();
	}

	#region API

	private readonly BingoSyncApiHandler _apiHandler;

	/// <summary>
	/// Creates a room from the given settings
	/// </summary>
	public async Task<bool> CreateRoom(CreateRoomSettings settings)
	{
		Log.Info($"Creating the room '{settings.Name}'...");

		var code = await _apiHandler.CreateRoom(settings);

		if (code == null)
		{
			Log.Error("Failed to create a room.");
			return false;
		}

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
	public async Task<bool> JoinRoom(JoinRoomSettings settings)
	{
		Log.Info($"Joining room '{settings.Code}'...");

		var socketKey = await _apiHandler.JoinRoom(settings);

		if (socketKey == null)
		{
			Log.Error($"Failed to join the room '{settings.Code}'.");
			return false;
		}

		Log.Info($"Room '{settings.Code}' was joined.");
		return await Connect(socketKey);
	}

	/// <summary>
	/// Leaves the room
	/// </summary>
	public async Task<bool> LeaveRoom()
	{
		if (!IsInRoom)
		{
			Log.Error("Tried to leave the room before being connected.");
			return false;
		}

		var roomId = RoomId;

		Log.Info($"Leaving the room '{roomId}'...");

		var hasDisconnected = await Disconnect();

		if (hasDisconnected)
		{
			Log.Info($"Left the room '{roomId}'.");
			return true;
		}

		Log.Error($"Failed to disconnected from the room '{roomId}'.");
		return false;
	}

	/// <inheritdoc cref="BingoSyncApiHandler.GetSquares(string)"/>
	public async Task<SquareData[]?> GetSquares()
	{
		if (!IsInRoom)
		{
			Log.Error("Tried to obtain the squares before being connected.");
			return null;
		}

		Log.Info($"Fetching the squares of the room '{RoomId}'...");

		var squares = await _apiHandler.GetSquares(RoomId);

		Log.Info($"Squares of the room '{RoomId}' was fetched.");
		return squares;
	}

	/// <summary>
	/// Changes the team of this client in the room
	/// </summary>
	public async Task<bool> ChangeTeam(Team newTeam)
	{
		if (!IsInRoom)
		{
			Log.Error("Tried to change team before being connected.");
			return false;
		}

		Log.Info($"Changing the team of the player '{UUID}' to '{newTeam}'...");

		var hasChanged = await _apiHandler.ChangeTeam(RoomId, newTeam);

		if (hasChanged)
		{
			Log.Info($"Changed the team of the player '{UUID}'.");
			return true;
		}

		Log.Error($"Failed to change team of the player '{UUID}'.");
		return false;
	}

	/// <summary>
	/// Marks the square at the given index for a certain team
	/// </summary>
	public async Task<bool> MarkSquare(Team team, int index)
	{
		if (!IsInRoom)
		{
			Log.Error("Tried to mark a square before being connected.");
			return false;
		}

		Log.Info($"Marking the square #{index} for the team '{team}'...");

		var hasMarked = await _apiHandler.MarkSquare(RoomId, team, index);

		if (hasMarked)
		{
			Log.Info($"Marked the square #{index} for the team '{team}'.");
			return true;
		}

		Log.Error($"Failed to mark the square #{index} for the team '{team}'.");
		return false;
	}

	/// <summary>
	/// Clears the square at the given index for a certain team
	/// </summary>
	public async Task<bool> ClearSquare(Team team, int index)
	{
		if (!IsInRoom)
		{
			Log.Error("Tried to clear a square before being connected.");
			return false;
		}

		Log.Info($"Clearing the square #{index} for the team '{team}'...");

		var hasCleared = await _apiHandler.ClearSquare(RoomId, team, index);

		if (hasCleared)
		{
			Log.Info($"Cleared the square #{index} for the team '{team}'.");
			return true;
		}

		Log.Error($"Failed to clear the square #{index} for the team '{team}'.");
		return false;
	}

	/// <summary>
	/// Sends a message in the room
	/// </summary>
	public async Task<bool> SendMessage(string message)
	{
		if (!IsInRoom)
		{
			Log.Error("Tried to send a message before being connected.");
			return false;
		}

		Log.Info($"Sending the following chat message as the player '{UUID}': '{message}'...");

		var hasSent = await _apiHandler.SendMessage(RoomId, message);

		if (hasSent)
		{
			Log.Info($"Sent the following chat message as the player '{UUID}': '{message}'.");
			return true;
		}

		Log.Error($"Failed to sent the following chat message as the player '{UUID}': '{message}'.");
		return false;
	}

	/// <summary>
	/// Reveals the card for the entire room
	/// </summary>
	public async Task<bool> RevealCard()
	{
		if (!IsInRoom)
		{
			Log.Error("Tried to reveal the card before being connected.");
			return false;
		}

		Log.Info($"Revealing the card in the room '{RoomId}' as the player '{UUID}'...");

		var hasRevealed = await _apiHandler.RevealCard(RoomId);

		if (hasRevealed)
		{
			Log.Info($"Revealed the card in the room '{RoomId}' as the player '{UUID}'.");
			return true;
		}

		Log.Error($"Failed to reveal the card in the room '{RoomId}' as the player '{UUID}'.");
		return false;
	}

	/// <summary>
	/// Gets the feed of every event in the room
	/// </summary>
	public async Task<BaseEvent[]> GetFeed()
	{
		if (!IsInRoom)
		{
			Log.Error("Tried to get the feed of the room before being connected.");
			return [];
		}

		Log.Info($"Fetching the feed of the room '{RoomId}'...");

		var events = await _apiHandler.GetFeed(RoomId);

		if (events == null)
		{
			Log.Error($"Failed to fetch the feed of the room '{RoomId}'.");
			return [];
		}

		Log.Info($"Fetched the feed of the room '{RoomId}'.");
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
		{
			Log.Error("Tried to connect to the server while being connected.");
			return false;
		}

		Log.Info("Connecting to the server...");

		var hasConnected = await _socketHandler.Connect(socketKey, _eventDispatcher.OnMessageReceived);

		if (hasConnected)
		{
			Log.Info("Connected to the server.");
			return true;
		}

		Log.Error("Failed to create the socket.");
		return false;
	}

	/// <summary>
	/// Disconnects this client from the servers
	/// </summary>
	/// <returns>Succeeded to disconnect</returns>
	protected virtual async Task<bool> Disconnect()
	{
		if (!IsInRoom)
		{
			Log.Error("Tried to disconnect from the server before being connected.");
			return false;
		}

		Log.Info("Disconnecting from the server...");

		await _socketHandler.Disconnect();

		Log.Info("Disconnected from the server.");

		RoomId = null;
		UUID = null;
		return true;
	}

	#endregion

	#region Events

	private readonly EventDispatcher _eventDispatcher;

	private void OnEventReceived(BaseEvent baseEvent)
	{
		if (!IsInRoom && baseEvent is ConnectedEvent connectedEvent)
		{
			RoomId = connectedEvent.RoomId;
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
