using System.Diagnostics.CodeAnalysis;
using BingoAPI.Events;
using BingoAPI.Goals;
using BingoAPI.Helpers;
using BingoAPI.Models;
using BingoAPI.Models.Settings;
using BingoAPI.Networking.Clients;
using JetBrains.Annotations;
using Newtonsoft.Json;

namespace BingoAPI.Networking;

/// <summary>
/// Represents an active connection to a room
/// </summary>
[PublicAPI]
public sealed class Session : IDisposable
{
	/// <summary>
	/// Options used to create a <see cref="Session"/>
	/// </summary>
	public record Options
	{
		/// <summary>
		/// Base of the URL for the web requests
		/// </summary>
		public string BaseUrl { get; init; } = "https://bingosync.com";

		/// <summary>
		/// Time required before a request is considered "Timed out"
		/// </summary>
		public TimeSpan Timeout { get; init; } = TimeSpan.FromSeconds(30);

		/// <summary>
		/// Creates a <see cref="HttpClient"/> from these options
		/// </summary>
		public HttpClient CreateClient()
		{
			var client = new HttpClient(
				new LoggingHandler(
					new HttpClientHandler()
				)
			);

			client.BaseAddress = new Uri(BaseUrl);
			client.Timeout = Timeout;

			return client;
		}
	}

	private readonly HttpClient _client;
	private readonly bool _shouldDisposeClient;

	private readonly BingoApiClient _api;
	private readonly BingoSocketClient _socket = new();

	private readonly EventDispatcher _dispatcher;

	private string? _roomCode;

	/// <summary>
	/// Team of the player
	/// </summary>
	public Team Team { get; private set; } = Team.None;

	/// <summary>
	/// Defines if this session is connected to a room
	/// </summary>
	[MemberNotNullWhen(true, nameof(_roomCode))]
	public bool IsInRoom => _roomCode != null;

	#region Constructors

	/// <summary>
	/// <inheritdoc cref="Session(BingoAPI.Events.EventDispatcher,System.Net.Http.HttpClient)"/>
	/// </summary>
	/// <param name="dispatcher"><inheritdoc cref="Session(BingoAPI.Events.EventDispatcher,System.Net.Http.HttpClient)"/></param>
	public Session(EventDispatcher dispatcher) : this(dispatcher, new Options()) { }

	/// <summary>
	/// <inheritdoc cref="Session(BingoAPI.Events.EventDispatcher,System.Net.Http.HttpClient)"/>
	/// </summary>
	/// <param name="dispatcher"><inheritdoc cref="Session(BingoAPI.Events.EventDispatcher,System.Net.Http.HttpClient)"/></param>
	/// <param name="options">Options from which to create a <see cref="HttpClient"/></param>
	public Session(EventDispatcher dispatcher, Options options) : this(dispatcher, options.CreateClient())
	{
		_shouldDisposeClient = true;
	}

	/// <summary>
	/// Creates a new instance of <see cref="Session"/>
	/// </summary>
	/// <param name="dispatcher">Instance used to dispatch events</param>
	/// <param name="client">Client used for HTTP requests</param>
	public Session(EventDispatcher dispatcher, HttpClient client)
	{
		_dispatcher = dispatcher;
		_client = client;
		_shouldDisposeClient = false;
		_api = new BingoApiClient(client);
	}

	#endregion

	/// <summary>
	/// Creates a room and joins it
	/// </summary>
	public async Task<bool> CreateRoom(CreateRoomSettings settings, CancellationToken ct = default)
	{
		throw new NotImplementedException();

		if (IsInRoom)
		{
			Log.Error("Tried to create a room while being connected.");
			return false;
		}

		Log.Info("Creating a room...");

		string code;

		try
		{
			code = await _api.CreateRoom(settings, ct);

			Log.Info($"Room created at '{code}'.");
		}
		catch (Exception e)
		{
			Log.Error($"Failed to create a room: {e}");
			return false;
		}

		Log.Info($"Joining room '{code}' from creation...");

		var joinSettings = new JoinRoomSettings
		{
			Code = code,
			Nickname = settings.Nickname,
			Password = settings.Password,
		};

		return await JoinRoom(joinSettings, ct);
	}

	/// <summary>
	/// Joins the room
	/// </summary>
	public async Task<bool> JoinRoom(JoinRoomSettings settings, CancellationToken ct = default)
	{
		if (IsInRoom)
		{
			Log.Error("Tried to join a room while being connected.");
			return false;
		}

		Log.Info($"Joining room '{settings.Code}'...");

		try
		{
			var socketKey = await _api.JoinRoom(settings, ct);

			await _socket.Connect(socketKey, OnMessageReceived, ct);

			var socketInfo = await _api.GetSocketInformation(socketKey, ct);

			_roomCode = socketInfo.Code;

			Team = BingoApiClient.DEFAULT_TEAM;

			var player = new Player
			{
				Name = settings.Nickname,
				Team = Team,
				UUID = socketInfo.PlayerUUID,
			};

			_dispatcher.DispatchConnect(player);

			Log.Info($"Room '{settings.Code}' was joined.");
			return true;
		}
		catch (Exception e)
		{
			Log.Error($"Failed to join the room '{settings.Code}': {e}");
			return false;
		}
	}

	/// <summary>
	/// Leaves the room
	/// </summary>
	public async Task<bool> LeaveRoom(CancellationToken ct = default)
	{
		if (!IsInRoom)
		{
			Log.Error("Tried to leave the room before being connected.");
			return false;
		}

		var room = _roomCode;

		Log.Info($"Leaving the room '{room}'...");

		try
		{
			await _socket.Disconnect(ct);

			_roomCode = null;
			_dispatcher.DispatchDisconnect();

			Log.Info($"Left the room '{room}'.");
			return true;
		}
		catch (Exception e)
		{
			Log.Error($"Failed to leave the room '{room}: {e}");
			return false;
		}
	}

	/// <summary>
	/// Sends a message in the room
	/// </summary>
	public async Task<bool> SendMessage(string message, CancellationToken ct = default)
	{
		if (!IsInRoom)
		{
			Log.Error("Tried to send a message before being connected.");
			return false;
		}

		Log.Info($"Sending the following chat message: '{message}'...");

		try
		{
			await _api.SendMessage(_roomCode, message, ct);

			Log.Info($"Sent the following chat message: '{message}'.");
			return true;
		}
		catch (Exception e)
		{
			Log.Error($"Failed to sent the chat message: {e}");
			return false;
		}
	}

	/// <summary>
	/// Changes the player's team
	/// </summary>
	public async Task<bool> ChangeTeam(Team team, CancellationToken ct = default)
	{
		if (!IsInRoom)
		{
			Log.Error("Tried to change team before being connected.");
			return false;
		}

		if (team == Team)
		{
			Log.Error("Tried to change to the same team.");
			return false;
		}

		Log.Info($"Changing team to '{team}'...");

		try
		{
			await _api.ChangeTeam(_roomCode, team, ct);

			Team = team;

			Log.Info($"Changed team to '{team}'.");
			return true;
		}
		catch (Exception e)
		{
			Log.Error($"Failed to change the team: {e}");
			return false;
		}
	}

	/// <summary>
	/// Gets the current <see cref="Card"/> of the room
	/// </summary>
	public async Task<Card?> GetCard(GoalPool pool, CancellationToken ct = default)
	{
		if (!IsInRoom)
		{
			Log.Error("Tried to get the squares before being connected.");
			return null;
		}

		Log.Info($"Getting the squares of the room '{_roomCode}'...");

		try
		{
			var squares = await _api.GetSquares(_roomCode, ct);

			Log.Info($"Got {squares.Length} squares for room '{_roomCode}'.");
			return new Card(squares, pool);
		}
		catch (Exception e)
		{
			Log.Error($"Failed to get squares for room '{_roomCode}': {e}");
			return null;
		}
	}

	/// <summary>
	/// Gets all the <see cref="Square"/> of the room
	/// </summary>
	public async Task<Square[]?> GetSquares(CancellationToken ct = default)
	{
		if (!IsInRoom)
		{
			Log.Error("Tried to get the squares before being connected.");
			return null;
		}

		Log.Info($"Getting the squares of the room '{_roomCode}'...");

		try
		{
			var squares = await _api.GetSquares(_roomCode, ct);

			Log.Info($"Got {squares.Length} squares for room '{_roomCode}'.");
			return squares;
		}
		catch (Exception e)
		{
			Log.Error($"Failed to get squares for room '{_roomCode}': {e}");
			return null;
		}
	}

	/// <summary>
	/// Marks the square for a team
	/// </summary>
	public async Task<bool> MarkSquare(int index, CancellationToken ct = default)
	{
		if (!IsInRoom)
		{
			Log.Error("Tried to mark a square before being connected.");
			return false;
		}

		if (Team == Team.None)
		{
			Log.Error("Tried to clear a square without being in a team.");
			return false;
		}

		Log.Info($"Marking the square #{index} for the team '{Team}'...");

		try
		{
			await _api.MarkSquare(
				_roomCode,
				Team,
				index,
				ct
			);

			Log.Info($"Marked the square #{index} for the team '{Team}'.");
			return true;
		}
		catch (Exception e)
		{
			Log.Error($"Failed to mark the square #{index} for the team '{Team}': {e}");
			return false;
		}
	}

	/// <summary>
	/// Clears the square for a team
	/// </summary>
	public async Task<bool> ClearSquare(int index, CancellationToken ct = default)
	{
		if (!IsInRoom)
		{
			Log.Error("Tried to clear a square before being connected.");
			return false;
		}

		if (Team == Team.None)
		{
			Log.Error("Tried to clear a square without being in a team.");
			return false;
		}

		Log.Info($"Clearing the square #{index} for the team '{Team}'...");

		try
		{
			await _api.ClearSquare(
				_roomCode,
				Team,
				index,
				ct
			);

			Log.Info($"Cleared the square #{index} for the team '{Team}'.");
			return true;
		}
		catch (Exception e)
		{
			Log.Error($"Failed to clear the square #{index} for the team '{Team}': {e}");
			return false;
		}
	}

	/// <summary>
	/// Reveals the card for the room
	/// </summary>
	public async Task<bool> RevealCard(CancellationToken ct = default)
	{
		if (!IsInRoom)
		{
			Log.Error("Tried to reveal the card before being connected.");
			return false;
		}

		Log.Info($"Revealing the card for the room '{_roomCode}'...");

		try
		{
			await _api.RevealCard(
				_roomCode,
				ct
			);

			Log.Info($"Revealed the card for the room '{_roomCode}'.");
			return true;
		}
		catch (Exception e)
		{
			Log.Error($"Failed to reveal the card for the room '{_roomCode}': {e}");
			return false;
		}
	}

	private void OnMessageReceived(string message)
	{
		var evt = JsonConvert.DeserializeObject<IEvent>(message);

		if (evt == null)
		{
			Log.Warning($"Failed to deserialize the message into a '{typeof(IEvent)}': {message}");
			return;
		}

		_dispatcher.Dispatch(evt);
	}

	/// <inheritdoc />
	public void Dispose()
	{
		if (_shouldDisposeClient)
			_client.Dispose();
		_socket.Dispose();
	}
}
