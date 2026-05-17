using BingoAPI.Events;
using BingoAPI.Helpers;
using BingoAPI.Models;
using BingoAPI.Models.Settings;
using BingoAPI.Networking;
using Newtonsoft.Json;

namespace BingoAPI.Session;

/// <summary>
/// Represents an active connection to a room
/// </summary>
public sealed class BingoSession : IDisposable
{
	private readonly BingoApiClient _api = new();
	private readonly BingoSocketClient _socket = new();

	public readonly EventDispatcher Events = new();

	private string? _roomCode;

	/// <summary>
	/// Joins the room with the given settings
	/// </summary>
	public async Task<bool> JoinRoom(JoinRoomSettings settings, CancellationToken ct = default)
	{
		if (_roomCode != null)
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

			Events.SetLocalPlayer(socketInfo.PlayerUUID);

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
		if (_roomCode == null)
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

			Log.Info($"Left the room '{room}'.");
			return true;
		}
		catch (Exception e)
		{
			Log.Error($"Failed to d_roomCode != null from the room '{room}': {e}");
			return false;
		}
	}

	/// <summary>
	/// Sends a message in the room
	/// </summary>
	public async Task<bool> SendMessage(string message, CancellationToken ct = default)
	{
		if (_roomCode == null)
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
	/// Changes the team of the room
	/// </summary>
	public async Task<bool> ChangeTeam(Team team, CancellationToken ct = default)
	{
		if (_roomCode == null)
		{
			Log.Error("Tried to change team before being connected.");
			return false;
		}

		Log.Info($"Changing team to '{team}'...");

		try
		{
			await _api.ChangeTeam(_roomCode, team, ct);

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
	/// Marks the square at the given index for a certain team
	/// </summary>
	public async Task<bool> MarkSquare(Team team, int index, CancellationToken ct = default)
	{
		if (_roomCode == null)
		{
			Log.Error("Tried to mark a square before being connected.");
			return false;
		}

		Log.Info($"Marking the square #{index} for the team '{team}'...");

		try
		{
			await _api.MarkSquare(
				_roomCode,
				team,
				index,
				ct
			);

			Log.Info($"Marked the square #{index} for the team '{team}'.");
			return true;
		}
		catch (Exception e)
		{
			Log.Error($"Failed to mark the square #{index} for the team '{team}': {e}");
			return false;
		}
	}

	/// <summary>
	/// Clears the square at the given index for a certain team
	/// </summary>
	public async Task<bool> ClearSquare(Team team, int index, CancellationToken ct = default)
	{
		if (_roomCode == null)
		{
			Log.Error("Tried to clear a square before being connected.");
			return false;
		}

		Log.Info($"Clearing the square #{index} for the team '{team}'...");

		try
		{
			await _api.ClearSquare(
				_roomCode,
				team,
				index,
				ct
			);

			Log.Info($"Cleared the square #{index} for the team '{team}'.");
			return true;
		}
		catch (Exception e)
		{
			Log.Error($"Failed to clear the square #{index} for the team '{team}': {e}");
			return false;
		}
	}

	/// <summary>
	/// Reveals the card in the room
	/// </summary>
	public async Task<bool> RevealCard(CancellationToken ct = default)
	{
		if (_roomCode == null)
		{
			Log.Error("Tried to reveal the card before being connected.");
			return false;
		}

		Log.Info($"Revealing the card in the room '{_roomCode}'...");

		try
		{
			await _api.RevealCard(
				_roomCode,
				ct
			);

			Log.Info($"Revealed the card in the room '{_roomCode}'.");
			return true;
		}
		catch (Exception e)
		{
			Log.Error($"Failed to reveal the card in the room '{_roomCode}': {e}");
			return false;
		}
	}

	/// <summary>
	/// Gets the setting of the room
	/// </summary>
	public async Task<RoomSettings?> GetRoomSettings(CancellationToken ct = default)
	{
		if (_roomCode == null)
		{
			Log.Error("Tried to get the room settings before being connected.");
			return null;
		}

		Log.Info($"Getting the room settings of the room '{_roomCode}'...");

		try
		{
			var settings = await _api.GetRoomSettings(_roomCode, ct);

			Log.Info($"Got the room settings of the room '{_roomCode}'.");
			return settings;
		}
		catch (Exception e)
		{
			Log.Error($"Failed to get the room settings of the room '{_roomCode}': {e}");
			return null;
		}
	}

	private void OnMessageReceived(string message)
	{
		var evt = JsonConvert.DeserializeObject<IBingoEvent>(message);

		if (evt == null)
		{
			Log.Warning($"Failed to deserialize the message into a '{typeof(IBingoEvent)}': {message}");
			return;
		}

		Events.Dispatch(evt);
	}

	/// <inheritdoc />
	public void Dispose()
	{
		_api.Dispose();
		_socket.Dispose();
	}
}
