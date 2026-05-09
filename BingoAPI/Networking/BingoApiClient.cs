using System.Net.WebSockets;
using BingoAPI.Extensions;
using BingoAPI.Models;
using BingoAPI.Networking.DTOs;

namespace BingoAPI.Networking;

/// <summary>
/// Handles all HTTP communication with the BingoSync REST API
/// </summary>
internal sealed class BingoApiClient : IDisposable
{
	private readonly BingoHttpClient _client = new();

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
	/// Gets the current board of the room
	/// </summary>
	public async Task<Board> GetBoard(string room)
	{
		var response = await _client.GetJson<ApiGetBoardItem[]>($"/room/{room}/board");

		var squares = new Square[response.Length];

		for (var i = 0; i < response.Length; i++)
		{
			var item = response[i];

			var slot = item.Slot.Replace("slot", "");

			if (!int.TryParse(slot, out var index))
				throw new InvalidOperationException($"Invalid slot '{slot}'");

			squares[i] = new Square(
				item.Name,
				index - 1,
				item.Colors.GetTeams()
			);
		}

		return new Board(squares);
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

	/// <inheritdoc />
	public void Dispose()
	{
		_client.Dispose();
	}
}
