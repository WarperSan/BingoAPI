using System.Net;
using System.Net.WebSockets;
using System.Text.RegularExpressions;
using BingoAPI.Helpers;
using BingoAPI.Models;
using BingoAPI.Models.Settings;
using BingoAPI.Networking.DTOs;
using Newtonsoft.Json;

namespace BingoAPI.Networking.Clients;

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
	private Task<HttpResponseMessage> Send(HttpRequestMessage request, CancellationToken ct) => _client.SendAsync(request, ct);

	/// <summary>
	/// Sends the given <see cref="HttpRequestMessage"/>
	/// </summary>
	private async Task SendAsync(HttpRequestMessage request, CancellationToken ct)
	{
		using var response = await Send(request, ct);
		response.EnsureSuccessStatusCode();
	}

	/// <summary>
	/// Sends the given <see cref="HttpRequestMessage"/>, and parses the JSON payload to <typeparamref name="T"/>
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

	#region Endpoints

	/// <summary>
	/// Gets the necessary tokens
	/// </summary>
	private async Task<Tokens> GetTokens(CancellationToken ct)
	{
		// ReSharper disable StringLiteralTypo
		const string PUBLIC_TOKEN = "csrftoken";
		const string CREATION_TOKEN = "csrfmiddlewaretoken";
		// ReSharper restore StringLiteralTypo

		using var request = new RequestBuilder()
							.Get()
							.ToEndpoint("")
							.Build();

		using var response = await Send(request, ct);
		response.EnsureSuccessStatusCode();

		var container = new CookieContainer();
		var setCookie = response.Headers.GetValues("Set-Cookie");

		foreach (var cookieHeader in setCookie)
			container.SetCookies(_client.BaseAddress, cookieHeader);

		var cookies = container.GetCookies(_client.BaseAddress);
		var publicTokenCookie = cookies[PUBLIC_TOKEN];

		if (publicTokenCookie == null)
			throw new KeyNotFoundException($"No cookie was set for '{PUBLIC_TOKEN}'.");

		var content = await response.Content.ReadAsStringAsync();

		var match = Regex.Match(
			content,
			$"<input[^>]*name=\"{CREATION_TOKEN}\"[^>]*value=\"(.*?)\"[^>]*>"
		);

		if (!match.Success)
			throw new KeyNotFoundException($"Could not find any input with '{CREATION_TOKEN}'.");

		return new Tokens
		{
			PublicToken = publicTokenCookie.Value,
			CreationToken = match.Groups[1].Value,
		};
	}

	/// <summary>
	/// Creates a room with the given settings
	/// </summary>
	/// <returns>Code of the room</returns>
	public async Task<string> CreateRoom(
		CreateRoomSettings settings,
		CancellationToken ct
	)
	{
		var tokens = await GetTokens(ct);

		var body = new CreateRoomRequest
		{
			RoomName = settings.Name,
			Password = settings.Password,
			Nickname = nameof(BingoAPI),
			IsLockout = settings.IsLockout,
			Seed = settings.Seed,
			IsRandomized = settings.IsRandomized,
			Board = "",
			CreationToken = tokens.CreationToken,
		};

		using var request = new RequestBuilder()
							.Post()
							.ToEndpoint("/")
							.WithForm(body)
							.Build();

		// ReSharper disable StringLiteralTypo
		request.Headers.Add("Cookie", $"csrftoken={tokens.PublicToken}");
		request.Headers.Add("X-CSRFToken", tokens.CreationToken);
		// ReSharper restore StringLiteralTypo

		using var response = await Send(request, ct);
		response.EnsureSuccessStatusCode();

		var url = request.RequestUri.ToString();

		if (!Network.TryGetRoomCode(url, out var code))
			throw new KeyNotFoundException($"Could not find room code from '{url}'.");

		return code;
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

	#endregion
}
