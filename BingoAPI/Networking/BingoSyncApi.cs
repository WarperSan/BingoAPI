using System.Net.WebSockets;
using BingoAPI.Models;
using BingoAPI.Networking.DTOs;

namespace BingoAPI.Networking;

/// <summary>
/// Handles all HTTP communication with the BingoSync REST API
/// </summary>
internal sealed class BingoSyncApi
{
	private readonly BingoHttpClient _client;

	public BingoSyncApi(BingoHttpClient client)
	{
		_client = client;
	}

	/// <summary>
	/// Joins the room with the given settings
	/// </summary>
	/// <returns>
	///	Socket key of the <see cref="WebSocket"/>
	/// </returns>
	public async Task<string> JoinRoom(JoinRoomSettings settings)
	{
		var body = new ApiJoinRoomRequest
		{
			Code = settings.Code,
			Password = settings.Password,
			Username = settings.Nickname,
			IsSpectator = settings.IsSpectator
		};

		var response = await _client.SendJson<ApiJoinRoomResponse>(
			HttpMethod.Post,
			"/api/join-room",
			body
		);

		return response.SocketKey;
	}

	/// <summary>
	/// Sends a message in the room
	/// </summary>
	public async Task SendMessage(string room, string message)
	{
		var body = new ApiSendMessageRequest
		{
			Code = room,
			Message = message
		};

		await _client.SendJson(
			HttpMethod.Put,
			"/api/chat",
			body
		);
	}
}
