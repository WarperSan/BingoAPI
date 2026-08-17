using System.Diagnostics.CodeAnalysis;
using BingoAPI.Configuration.Settings;
using BingoAPI.Events;
using BingoAPI.Goals;
using BingoAPI.Logging;
using BingoAPI.Models;
using Newtonsoft.Json.Linq;

namespace BingoAPI.Clients.BingoSync;

/// <summary>
/// Default implementation of <see cref="IBingoSessionClient"/> for BingoSync
/// </summary>
public class BingoSyncSessionClient : IBingoSessionClient
{
	private string? _roomCode;

	private readonly IBingoApiClient _apiClient;
	private readonly IBingoSocketClient _socketClient;
	private readonly IEventHandler _eventHandler;
	private readonly IEventProvider _eventProvider;
	private readonly ILogger _logger;

	/// <inheritdoc />
	public Team Team { get; private set; } = Team.None;

	/// <inheritdoc />
	[MemberNotNullWhen(true, nameof(_roomCode))]
	public bool IsInRoom => _roomCode != null;

	/// <summary>
	/// Initializes a new instance of the <see cref="BingoSyncSessionClient"/> class.
	/// </summary>
	public BingoSyncSessionClient(
		IBingoApiClient apiClient,
		IBingoSocketClient socketClient,
		IEventHandler eventHandler,
		IEventProvider eventProvider,
		ILogger logger
	)
	{
		_apiClient = apiClient;
		_socketClient = socketClient;
		_eventHandler = eventHandler;
		_eventProvider = eventProvider;
		_logger = logger;
	}

	/// <inheritdoc />
	public async Task<bool> CreateRoom(CreateRoomSettings settings, CancellationToken ct = default)
	{
		if (IsInRoom)
		{
			_logger.Warning("Tried to create a room while being connected.");
			return false;
		}

		_logger.Info("Creating a room...");

		string code;

		try
		{
			code = await _apiClient.CreateRoom(settings, ct);

			_logger.Info($"Room created at '{code}'.");
		}
		catch (Exception e)
		{
			_logger.Error($"Failed to create a room: {e}");
			return false;
		}

		_logger.Info($"Joining room '{code}' from creation...");

		var joinSettings = new JoinRoomSettings
		{
			Code = code,
			Nickname = settings.Nickname,
			Password = settings.Password,
		};

		return await JoinRoom(joinSettings, ct);
	}

	/// <inheritdoc />
	public async Task<bool> JoinRoom(JoinRoomSettings settings, CancellationToken ct = default)
	{
		if (IsInRoom)
		{
			_logger.Warning("Tried to join a room while being connected.");
			return false;
		}

		_logger.Info($"Joining room '{settings.Code}'...");

		try
		{
			var socketKey = await _apiClient.JoinRoom(settings, ct);

			await _socketClient.Connect(socketKey, OnMessageReceived, ct);

			var socketInfo = await _apiClient.GetSocketIdentity(socketKey, ct);

			_roomCode = socketInfo.Code;

			// TODO: Team = _apiClient.DefaultTeam;
			Team = Team.Red;

			var player = new Player
			{
				Name = settings.Nickname,
				Team = Team,
				UUID = socketInfo.PlayerUUID,
			};

			_eventHandler.HandleConnect(player);

			_logger.Info($"Room '{settings.Code}' was joined.");
			return true;
		}
		catch (Exception e)
		{
			_logger.Error($"Failed to join the room '{settings.Code}': {e}");
			return false;
		}
	}

	/// <inheritdoc />
	public async Task<bool> LeaveRoom(CancellationToken ct = default)
	{
		if (!IsInRoom)
		{
			_logger.Warning("Tried to leave the room before being connected.");
			return false;
		}

		var room = _roomCode;

		_logger.Info($"Leaving the room '{room}'...");

		try
		{
			await _socketClient.Disconnect(ct);

			_roomCode = null;
			_eventHandler.HandleDisconnect();

			_logger.Info($"Left the room '{room}'.");
			return true;
		}
		catch (Exception e)
		{
			_logger.Error($"Failed to leave the room '{room}: {e}");
			return false;
		}
	}

	/// <inheritdoc />
	public async Task<bool> SendMessage(string message, CancellationToken ct = default)
	{
		if (!IsInRoom)
		{
			_logger.Warning("Tried to send a message before being connected.");
			return false;
		}

		_logger.Info($"Sending the following chat message: '{message}'...");

		try
		{
			await _apiClient.SendMessage(_roomCode, message, ct);

			_logger.Info($"Sent the following chat message: '{message}'.");
			return true;
		}
		catch (Exception e)
		{
			_logger.Error($"Failed to sent the chat message: {e}");
			return false;
		}
	}

	/// <inheritdoc />
	public async Task<bool> ChangeTeam(Team team, CancellationToken ct = default)
	{
		if (!IsInRoom)
		{
			_logger.Warning("Tried to change team before being connected.");
			return false;
		}

		if (team == Team)
		{
			_logger.Warning("Tried to change to the same team.");
			return false;
		}

		_logger.Info($"Changing team to '{team}'...");

		try
		{
			await _apiClient.ChangeTeam(_roomCode, team, ct);

			Team = team;

			_logger.Info($"Changed team to '{team}'.");
			return true;
		}
		catch (Exception e)
		{
			_logger.Error($"Failed to change the team: {e}");
			return false;
		}
	}

	/// <inheritdoc />
	public async Task<Card?> GetCard(IGoalPool pool, CancellationToken ct = default)
	{
		if (!IsInRoom)
		{
			_logger.Error("Tried to get the squares before being connected.");
			return null;
		}

		_logger.Info($"Getting the squares of the room '{_roomCode}'...");

		try
		{
			var squares = await _apiClient.GetSquares(_roomCode, ct);

			_logger.Info($"Got {squares.Count} squares for room '{_roomCode}'.");
			return new Card(squares, pool, _logger);
		}
		catch (Exception e)
		{
			_logger.Error($"Failed to get squares for room '{_roomCode}': {e}");
			return null;
		}
	}

	/// <inheritdoc />
	public async Task<bool> MarkSquare(int index, CancellationToken ct = default)
	{
		if (!IsInRoom)
		{
			_logger.Warning("Tried to mark a square before being connected.");
			return false;
		}

		if (Team == Team.None)
		{
			_logger.Warning("Tried to clear a square without being in a team.");
			return false;
		}

		_logger.Info($"Marking the square #{index} for the team '{Team}'...");

		try
		{
			await _apiClient.MarkSquare(_roomCode, Team, index, ct);

			_logger.Info($"Marked the square #{index} for the team '{Team}'.");
			return true;
		}
		catch (Exception e)
		{
			_logger.Error($"Failed to mark the square #{index} for the team '{Team}': {e}");
			return false;
		}
	}

	/// <inheritdoc />
	public async Task<bool> ClearSquare(int index, CancellationToken ct = default)
	{
		if (!IsInRoom)
		{
			_logger.Warning("Tried to clear a square before being connected.");
			return false;
		}

		if (Team == Team.None)
		{
			_logger.Warning("Tried to clear a square without being in a team.");
			return false;
		}

		_logger.Info($"Clearing the square #{index} for the team '{Team}'...");

		try
		{
			await _apiClient.ClearSquare(_roomCode, Team, index, ct);

			_logger.Info($"Cleared the square #{index} for the team '{Team}'.");
			return true;
		}
		catch (Exception e)
		{
			_logger.Error($"Failed to clear the square #{index} for the team '{Team}': {e}");
			return false;
		}
	}

	/// <inheritdoc />
	public async Task<bool> RevealCard(CancellationToken ct = default)
	{
		if (!IsInRoom)
		{
			_logger.Warning("Tried to reveal the card before being connected.");
			return false;
		}

		_logger.Info($"Revealing the card for the room '{_roomCode}'...");

		try
		{
			await _apiClient.RevealCard(_roomCode, ct);

			_logger.Info($"Revealed the card for the room '{_roomCode}'.");
			return true;
		}
		catch (Exception e)
		{
			_logger.Error($"Failed to reveal the card for the room '{_roomCode}': {e}");
			return false;
		}
	}

	private void OnMessageReceived(string message)
	{
		var obj = JObject.Parse(message);
		var evt = _eventProvider.Create(obj);

		_eventHandler.Handle(evt);
	}
}
