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
internal sealed class BingoApiClient
{
	/// <summary>
	/// Initial team when joining
	/// </summary>
	public const Team DEFAULT_TEAM = Team.Red;

	// TODO: CreateRoom, NewCard

	private readonly HttpClient _client;

	public BingoApiClient(HttpClient client)
	{
		_client = client;
	}

	#region Helpers

	/// <summary>
	/// Sends the given <see cref="HttpRequestMessage"/>
	/// </summary>
	private async Task SendAsync(HttpRequestMessage request, CancellationToken ct)
	{
		using var response = await _client.SendAsync(request, ct);
		response.EnsureSuccessStatusCode();
	}

	/// <summary>
	/// Sensd the given <see cref="HttpRequestMessage"/>, and parses the JSON payload to <see cref="T"/>
	/// </summary>
	private async Task<T> SendAndParse<T>(HttpRequestMessage request, CancellationToken ct)
	{
		using var response = await _client.SendAsync(request, ct);
		response.EnsureSuccessStatusCode();

		var responseBody = await response.Content.ReadAsStringAsync();
		var typedResponse = JsonConvert.DeserializeObject<T>(responseBody);

		// ReSharper disable once ConvertIfStatementToReturnStatement
		if (typedResponse == null)
			throw new InvalidOperationException($"Failed to deserialize response to {typeof(T).Name}");

		return typedResponse;
	}

	#endregion

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

		using var request = new RequestBuilder()
							.Post()
							.ToEndpoint("/api/join-room")
							.WithJson(body)
							.Build();

		var response = await SendAndParse<JoinRoomResponse>(request, ct);

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

		using var request = new RequestBuilder()
							.Put()
							.ToEndpoint("/api/select")
							.WithJson(body)
							.Build();

		await SendAsync(request, ct);
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

		using var request = new RequestBuilder()
							.Put()
							.ToEndpoint("/api/select")
							.WithJson(body)
							.Build();

		await SendAsync(request, ct);
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

		using var request = new RequestBuilder()
							.Put()
							.ToEndpoint("/api/chat")
							.WithJson(body)
							.Build();

		await SendAsync(request, ct);
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

		using var request = new RequestBuilder()
							.Put()
							.ToEndpoint("/api/color")
							.WithJson(body)
							.Build();

		await SendAsync(request, ct);
	}

	/// <summary>
	/// Gets all the squares of the room
	/// </summary>
	public async Task<Square[]> GetSquares(string room, CancellationToken ct)
	{
		using var request = new RequestBuilder()
							.Get()
							.ToEndpoint($"/room/{room}/board")
							.Build();

		return await SendAndParse<Square[]>(request, ct);
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

		using var request = new RequestBuilder()
							.Put()
							.ToEndpoint("/api/revealed")
							.WithJson(body)
							.Build();

		await SendAsync(request, ct);
	}

	/// <summary>
	/// Gets the feed of <see cref="IBingoEvent"/> in the room
	/// </summary>
	public async Task<IBingoEvent[]> GetFeed(string room, CancellationToken ct)
	{
		using var request = new RequestBuilder()
							.Get()
							.ToEndpoint($"/room/{room}/feed")
							.Build();

		var response = await SendAndParse<GetFeedResponse>(request, ct);

		return response.Events;
	}

	/// <summary>
	/// Gets the information related to the given socket key
	/// </summary>
	public async Task<GetSocketInformationResponse> GetSocketInformation(string socketKey, CancellationToken ct)
	{
		using var request = new RequestBuilder()
							.Get()
							.ToEndpoint($"/api/socket/{socketKey}")
							.Build();

		return await SendAndParse<GetSocketInformationResponse>(request, ct);
	}

	/// <summary>
	/// Gets the settings of the room
	/// </summary>
	public async Task<RoomSettings> GetRoomSettings(string room, CancellationToken ct)
	{
		using var request = new RequestBuilder()
							.Get()
							.ToEndpoint($"/room/{room}/room-settings")
							.Build();

		var response = await SendAndParse<GetRoomSettingsResponse>(request, ct);

		return response.Settings;
	}
}
