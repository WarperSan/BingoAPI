using System.Net.Http.Headers;
using System.Net.WebSockets;
using BingoAPI.Events;
using BingoAPI.Models;
using BingoAPI.Models.Settings;
using BingoAPI.Networking.DTOs;
using Newtonsoft.Json;

namespace BingoAPI.Networking;

/// <summary>
/// Handles all HTTP communication with the BingoSync REST API
/// </summary>
internal sealed class BingoApiClient : IDisposable
{
	// TODO: CreateRoom, NewCard

	public BingoApiClient()
	{
		_client = new HttpClient(
			new LoggingHandler(
				new HttpClientHandler()
			)
		);
		_client.Timeout = TimeSpan.FromSeconds(30);

		_client.DefaultRequestHeaders.UserAgent.Add(new ProductInfoHeaderValue(
			"BingoAPI",
			"1.0.0"
		));

		_builder = new RequestBuilder()
			.ToUri(new Uri("https://bingosync.com"));
	}

	#region HTTP

	private readonly HttpClient _client;

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

	#endregion

	#region API

	private readonly RequestBuilder _builder;

	/// <summary>
	/// Joins the room with the given settings
	/// </summary>
	/// <returns>
	///	Socket key of the <see cref="WebSocket"/>
	/// </returns>
	public async Task<string> JoinRoom(
		JoinRoomSettings settings,
		CancellationToken ct
	)
	{
		var body = new JoinRoomRequest
		{
			Code = settings.Code,
			Password = settings.Password,
			Username = settings.Nickname,
			IsSpectator = settings.IsSpectator,
		};

		using var request = new RequestBuilder(_builder)
							.Post()
							.ToEndpoint("/api/join-room")
							.WithJson(body)
							.Build();

		using var responseMessage = await _client.SendAsync(request, ct);
		responseMessage.EnsureSuccessStatusCode();

		var response = await ParseJson<JoinRoomResponse>(responseMessage);

		return response.SocketKey;
	}

	/// <summary>
	/// Marks the square at the given index for a certain team
	/// </summary>
	public async Task MarkSquare(
		string room,
		Team team,
		int index,
		CancellationToken ct
	)
	{
		var body = new MarkSquareRequest
		{
			Code = room,
			Team = team,
			Index = (index + 1).ToString(),
		};

		using var request = new RequestBuilder(_builder)
							.Put()
							.ToEndpoint("/api/select")
							.WithJson(body)
							.Build();

		using var response = await _client.SendAsync(request, ct);
		response.EnsureSuccessStatusCode();
	}

	/// <summary>
	/// Clears the square at the given index for a certain team
	/// </summary>
	public async Task ClearSquare(
		string room,
		Team team,
		int index,
		CancellationToken ct
	)
	{
		var body = new ClearSquareRequest
		{
			Code = room,
			Team = team,
			Index = (index + 1).ToString(),
		};

		using var request = new RequestBuilder(_builder)
							.Put()
							.ToEndpoint("/api/select")
							.WithJson(body)
							.Build();

		using var responseMessage = await _client.SendAsync(request, ct);
		responseMessage.EnsureSuccessStatusCode();
	}

	/// <summary>
	/// Sends a message in the room
	/// </summary>
	public async Task SendMessage(string room, string message, CancellationToken ct)
	{
		var body = new SendMessageRequest
		{
			Code = room,
			Message = message,
		};

		using var request = new RequestBuilder(_builder)
							.Put()
							.ToEndpoint("/api/chat")
							.WithJson(body)
							.Build();

		using var responseMessage = await _client.SendAsync(request, ct);
		responseMessage.EnsureSuccessStatusCode();
	}

	/// <summary>
	/// Changes the team of the client in the room
	/// </summary>
	public async Task ChangeTeam(string room, Team team, CancellationToken ct)
	{
		var body = new ChangeTeamRequest
		{
			Code = room,
			Team = team,
		};

		using var request = new RequestBuilder(_builder)
							.Put()
							.ToEndpoint("/api/color")
							.WithJson(body)
							.Build();

		using var responseMessage = await _client.SendAsync(request, ct);
		responseMessage.EnsureSuccessStatusCode();
	}

	/// <summary>
	/// Reveals the card in the room
	/// </summary>
	public async Task RevealCard(string room, CancellationToken ct)
	{
		var body = new RevealCardRequest
		{
			Code = room,
		};

		using var request = new RequestBuilder(_builder)
							.Put()
							.ToEndpoint("/api/revealed")
							.WithJson(body)
							.Build();

		using var responseMessage = await _client.SendAsync(request, ct);
		responseMessage.EnsureSuccessStatusCode();
	}

	/// <summary>
	/// Gets the feed of <see cref="IBingoEvent"/> in the room
	/// </summary>
	public async Task<IBingoEvent[]> GetFeed(string room, CancellationToken ct)
	{
		using var request = new RequestBuilder(_builder)
							.Get()
							.ToEndpoint($"/room/{room}/feed")
							.Build();

		using var responseMessage = await _client.SendAsync(request, ct);
		responseMessage.EnsureSuccessStatusCode();

		var response = await ParseJson<GetFeedResponse>(responseMessage);

		return response.Events;
	}

	#endregion

	/// <inheritdoc />
	public void Dispose()
	{
		_client.Dispose();
	}
}
