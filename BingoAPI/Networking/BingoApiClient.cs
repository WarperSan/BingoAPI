using System.Net.Http.Headers;
using System.Net.WebSockets;
using BingoAPI.Events;
using BingoAPI.Helpers;
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

	private readonly HttpClient _client;
	private readonly RequestBuilder _builder;

	public BingoApiClient()
	{
		_client = new HttpClient();
		_client.Timeout = TimeSpan.FromSeconds(30);

		_client.DefaultRequestHeaders.UserAgent.Add(new ProductInfoHeaderValue(
			"BingoAPI",
			"1.0.0"
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

	private async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken ct)
	{
		Log.Debug($"""
		           {request.Method} {request.RequestUri?.PathAndQuery} HTTP/{request.Version}
		           Host: {request.RequestUri?.Host}
		           {request.Headers}

		           {await request.Content.ReadAsStringAsync()}
		           """);

		var response = await _client.SendAsync(request, ct);

		Log.Debug($"""
		           HTTP/{response.Version} {(int)response.StatusCode} {response.ReasonPhrase}
		           {response.Headers}

		           {await response.Content.ReadAsStringAsync()}
		           """);

		return response;
	}

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
		var body = new ApiJoinRoomRequest
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

		using var responseMessage = await SendAsync(request, ct);
		responseMessage.EnsureSuccessStatusCode();

		var response = await ParseJson<ApiJoinRoomResponse>(responseMessage);

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
		var body = new ApiMarkSquareRequest
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

		using var response = await SendAsync(request, ct);
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
		var body = new ApiClearSquareRequest
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

		using var responseMessage = await SendAsync(request, ct);
		responseMessage.EnsureSuccessStatusCode();
	}

	/// <summary>
	/// Sends a message in the room
	/// </summary>
	public async Task SendMessage(string room, string message, CancellationToken ct)
	{
		var body = new ApiSendMessageRequest
		{
			Code = room,
			Message = message,
		};

		using var request = new RequestBuilder(_builder)
							.Put()
							.ToEndpoint("/api/chat")
							.WithJson(body)
							.Build();

		using var responseMessage = await SendAsync(request, ct);
		responseMessage.EnsureSuccessStatusCode();
	}

	/// <summary>
	/// Changes the team of the client in the room
	/// </summary>
	public async Task ChangeTeam(string room, Team team, CancellationToken ct)
	{
		var body = new ApiChangeTeamRequest
		{
			Code = room,
			Team = team,
		};

		using var request = new RequestBuilder(_builder)
							.Put()
							.ToEndpoint("/api/color")
							.WithJson(body)
							.Build();

		using var responseMessage = await SendAsync(request, ct);
		responseMessage.EnsureSuccessStatusCode();
	}

	/// <summary>
	/// Reveals the card in the room
	/// </summary>
	public async Task RevealCard(string room, CancellationToken ct)
	{
		var body = new ApiRevealCardRequest
		{
			Code = room,
		};

		using var request = new RequestBuilder(_builder)
							.Put()
							.ToEndpoint("/api/revealed")
							.WithJson(body)
							.Build();

		using var responseMessage = await SendAsync(request, ct);
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

		using var responseMessage = await SendAsync(request, ct);
		responseMessage.EnsureSuccessStatusCode();

		return await ParseJson<IBingoEvent[]>(responseMessage);
	}

	/// <inheritdoc />
	public void Dispose()
	{
		_client.Dispose();
	}
}
