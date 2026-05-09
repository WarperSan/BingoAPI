using System.Diagnostics.CodeAnalysis;
using BingoAPI.Helpers;
using BingoAPI.Models;
using BingoAPI.Networking;

namespace BingoAPI.Session;

/// <summary>
/// Represents an active connection to a BingoSync room.
/// </summary>
public sealed class BingoSession : IDisposable
{
	private readonly BingoApiClient _api;
	private readonly BingoSocketClient _socket;

	public string? RoomId { get; private set; }
	public string? UUID { get; private set; }

	[MemberNotNullWhen(true, nameof(RoomId), nameof(UUID))]
	public bool IsConnected => RoomId != null && UUID != null;

	public BingoSession()
	{
		_api = new BingoApiClient();
		_socket = new BingoSocketClient();
	}

	/// <summary>
	/// Joins the room with the given settings
	/// </summary>
	public async Task<bool> JoinRoom(JoinRoomSettings settings)
	{
		Log.Info($"Joining room '{settings.Code}'...");

		try
		{
			var socketKey = await _api.JoinRoom(settings);

			await _socket.Connect(socketKey, Log.Info);

			// TODO: Make it wired with events
			RoomId = settings.Code;
			UUID = settings.Nickname;

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
	public async Task<bool> LeaveRoom()
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
			await _socket.Disconnect();

			RoomId = null;
			UUID = null;

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
	public async Task<bool> SendMessage(string message)
	{
		if (!IsConnected)
		{
			Log.Error("Tried to send a message before being connected.");
			return false;
		}

		Log.Info($"Sending the following chat message as the player '{UUID}': '{message}'...");

		try
		{
			await _api.SendMessage(RoomId, message);

			Log.Info($"Sent the following chat message as the player '{UUID}': '{message}'.");
			return true;
		}
		catch (Exception e)
		{
			Log.Error($"Failed to sent the chat message as the player '{UUID}': {e.Message}");
			return false;
		}
	}

	/// <inheritdoc />
	public void Dispose()
	{
		_api.Dispose();
		_socket.Dispose();
	}
}
