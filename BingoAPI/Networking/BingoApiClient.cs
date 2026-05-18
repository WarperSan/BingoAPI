using System.Net.Http.Headers;
using System.Net.WebSockets;
using BingoAPI.Events;
using BingoAPI.Extensions;
using BingoAPI.Models;
using BingoAPI.Models.Settings;
using BingoAPI.Networking.DTOs;

namespace BingoAPI.Networking;

/// <summary>
/// Handles all HTTP communication with the BingoSync REST API
/// </summary>
internal sealed class BingoApiClient : IDisposable
{
	/// <summary>
	/// Initial team when joining
	/// </summary>
	internal const Team DEFAULT_TEAM = Team.Red;

	// TODO: CreateRoom, NewCard

	private readonly HttpClient _client;
	private readonly RequestBuilder _builder;

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

		var response = await responseMessage.ParseJson<JoinRoomResponse>();

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
	/// Gets all the squares of the room
	/// </summary>
	public async Task<Square[]> GetSquares(string room, CancellationToken ct)
	{
		using var request = new RequestBuilder(_builder)
							.Get()
							.ToEndpoint($"/room/{room}/board")
							.Build();

		using var responseMessage = await _client.SendAsync(request, ct);
		responseMessage.EnsureSuccessStatusCode();

		return await responseMessage.ParseJson<Square[]>();
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

		var response = await responseMessage.ParseJson<GetFeedResponse>();

		return response.Events;
	}

	/// <summary>
	/// Gets the information related to the given socket key
	/// </summary>
	public async Task<GetSocketInformationResponse> GetSocketInformation(string socketKey, CancellationToken ct)
	{
		using var request = new RequestBuilder(_builder)
							.Get()
							.ToEndpoint($"/api/socket/{socketKey}")
							.Build();

		using var responseMessage = await _client.SendAsync(request, ct);
		responseMessage.EnsureSuccessStatusCode();

		return await responseMessage.ParseJson<GetSocketInformationResponse>();
	}

	/// <summary>
	/// Gets the settings of the room
	/// </summary>
	public async Task<RoomSettings> GetRoomSettings(string room, CancellationToken ct)
	{
		using var request = new RequestBuilder(_builder)
							.Get()
							.ToEndpoint($"/room/{room}/room-settings")
							.Build();

		using var responseMessage = await _client.SendAsync(request, ct);
		responseMessage.EnsureSuccessStatusCode();

		var response = await responseMessage.ParseJson<GetRoomSettingsResponse>();

		return response.Settings;
	}

	/// <inheritdoc />
	public void Dispose()
	{
		_client.Dispose();
	}
}
