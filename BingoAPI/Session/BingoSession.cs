using System.Diagnostics.CodeAnalysis;
using BingoAPI.Events;
using BingoAPI.Helpers;
using BingoAPI.Models;
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

	public string? RoomId { get; private set; }

	public Player? Player { get; set; }

	[MemberNotNullWhen(true, nameof(RoomId), nameof(Player))]
	public bool IsConnected => RoomId != null && Player != null;

	/// <summary>
	/// Joins the room with the given settings
	/// </summary>
	public async Task<bool> JoinRoom(JoinRoomSettings settings, CancellationToken ct = default)
	{
		Log.Info($"Joining room '{settings.Code}'...");

		try
		{
			var socketKey = await _api.JoinRoom(settings, ct);

			await _socket.Connect(socketKey, OnMessageReceived, ct);

			Log.Info($"Room '{settings.Code}' was joined.");
			return true;
		}
		catch (Exception e)
		{
			Log.Error($"Failed to join the room '{settings.Code}': {e.Message}");
			return false;
		}
	}

	/// <summary>
	/// Leaves the room
	/// </summary>
	public async Task<bool> LeaveRoom(CancellationToken ct = default)
	{
		if (!IsConnected)
		{
			Log.Error("Tried to leave the room before being connected.");
			return false;
		}

		var roomId = RoomId;

		Log.Info($"Leaving the room '{roomId}'...");

		try
		{
			await _socket.Disconnect(ct);

			RoomId = null;
			Player = null;

			Log.Info($"Left the room '{roomId}'.");
			return true;
		}
		catch (Exception e)
		{
			Log.Error($"Failed to disconnected from the room '{roomId}': {e.Message}");
			return false;
		}
	}

	/// <summary>
	/// Sends a message in the room
	/// </summary>
	public async Task<bool> SendMessage(string message, CancellationToken ct = default)
	{
		if (!IsConnected)
		{
			Log.Error("Tried to send a message before being connected.");
			return false;
		}

		Log.Info($"Sending the following chat message as the player '{Player.UUID}': '{message}'...");

		try
		{
			await _api.SendMessage(RoomId, message, ct);

			Log.Info($"Sent the following chat message as the player '{Player.UUID}': '{message}'.");
			return true;
		}
		catch (Exception e)
		{
			Log.Error($"Failed to sent the chat message as the player '{Player.UUID}': {e.Message}");
			return false;
		}
	}

	/// <summary>
	/// Changes the team of the room
	/// </summary>
	public async Task<bool> ChangeTeam(Team team, CancellationToken ct = default)
	{
		if (!IsConnected)
		{
			Log.Error("Tried to change team before being connected.");
			return false;
		}

		Log.Info($"Changing team to '{team}' as the player '{Player.UUID}'...");

		try
		{
			await _api.ChangeTeam(RoomId, team, ct);

			Log.Info($"Changed team to '{team}' as the player '{Player.UUID}'.");
			return true;
		}
		catch (Exception e)
		{
			Log.Error($"Failed to change the team as the player '{Player.UUID}': {e.Message}");
			return false;
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

		if (!IsConnected && evt is ConnectionEvent e && e.IsConnected)
		{
			RoomId = e.RoomId;
			Player = e.Player;
			Events.SetLocalPlayer(Player);
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
