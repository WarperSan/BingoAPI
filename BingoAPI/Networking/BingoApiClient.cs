using System.Net.Http.Headers;
using System.Net.WebSockets;
using BingoAPI.Extensions;
using BingoAPI.Models;
using BingoAPI.Networking.DTOs;
using Newtonsoft.Json;

namespace BingoAPI.Networking;

/// <summary>
/// Handles all HTTP communication with the BingoSync REST API
/// </summary>
internal sealed class BingoApiClient : IDisposable
{
	private readonly HttpClient _client;
	private readonly RequestBuilder _builder;

	public BingoApiClient()
	{
		_client = new HttpClient();
		_client.Timeout = TimeSpan.FromSeconds(30);

		_client.DefaultRequestHeaders.UserAgent.Add(new ProductInfoHeaderValue(
			Plugin.Id,
			Plugin.Version
		));

		_builder = new RequestBuilder()
			.ToUri(new Uri("https://bingosync.com"));
	}

	/// <summary>
	/// Parses the JSON response
	/// </summary>
	private static async Task<T> ParseJson<T>(HttpResponseMessage response)
	{
		var responseBody = await response.Content.ReadAsStringAsync();
		var typedResponse = JsonConvert.DeserializeObject<T>(responseBody);

		// ReSharper disable once ConvertIfStatementToReturnStatement
		if (typedResponse == null)
			throw new InvalidOperationException($"Failed to deserialize response to {typeof(T).Name}");

		return typedResponse;
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

		using var request = new RequestBuilder(_builder)
							.Post()
							.ToEndpoint("/api/join-room")
							.WithJson(body)
							.Build();

		using var responseMessage = await _client.SendAsync(request);
		responseMessage.EnsureSuccessStatusCode();

		var response = await ParseJson<ApiJoinRoomResponse>(responseMessage);

		return response.SocketKey;
	}

	/// <summary>
	/// Gets the current board of the room
	/// </summary>
	public async Task<Board> GetBoard(string room)
	{
		using var request = new RequestBuilder(_builder)
							.Get()
							.ToEndpoint($"/room/{room}/board")
							.Build();

		using var responseMessage = await _client.SendAsync(request);
		responseMessage.EnsureSuccessStatusCode();

		var response = await ParseJson<ApiGetBoardItem[]>(responseMessage);

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
				item.Colors.FromColorString()
			);
		}

		return new Board(squares);
	}

	/// <summary>
	/// Marks the square at the given index for a certain team
	/// </summary>
	public async Task MarkSquare(string room, Team team, int index)
	{
		var body = new ApiMarkSquareRequest
		{
			Code = room,
			Team = team.ToColorString(),
			Index = (index + 1).ToString()
		};

		using var request = new RequestBuilder(_builder)
							.Put()
							.ToEndpoint("/api/select")
							.WithJson(body)
							.Build();

		using var response = await _client.SendAsync(request);
		response.EnsureSuccessStatusCode();
	}

	/// <summary>
	/// Clears the square at the given index for a certain team
	/// </summary>
	public async Task ClearSquare(string room, Team team, int index)
	{
		var body = new ApiClearSquareRequest
		{
			Code = room,
			Team = team.ToColorString(),
			Index = (index + 1).ToString()
		};

		using var request = new RequestBuilder(_builder)
							.Put()
							.ToEndpoint("/api/select")
							.WithJson(body)
							.Build();

		using var responseMessage = await _client.SendAsync(request);
		responseMessage.EnsureSuccessStatusCode();
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

		using var request = new RequestBuilder(_builder)
							.Put()
							.ToEndpoint("/api/chat")
							.WithJson(body)
							.Build();

		using var responseMessage = await _client.SendAsync(request);
		responseMessage.EnsureSuccessStatusCode();
	}

	/// <summary>
	/// Changes the team of the client in the room
	/// </summary>
	public async Task ChangeTeam(string room, Team team)
	{
		var body = new ApiChangeTeamRequest
		{
			Code = room,
			Team = team.ToColorString()
		};

		using var request = new RequestBuilder(_builder)
							.Put()
							.ToEndpoint("/api/color")
							.WithJson(body)
							.Build();

		using var responseMessage = await _client.SendAsync(request);
		responseMessage.EnsureSuccessStatusCode();
	}

	/// <inheritdoc />
	public void Dispose()
	{
		_client.Dispose();
	}
}
