using System.Net;
using System.Text.RegularExpressions;
using BingoAPI.Clients.Interfaces;
using BingoAPI.DTOs.CreateRoom;
using BingoAPI.Helpers;
using BingoAPI.Models;
using BingoAPI.Models.Settings;
using BingoAPI.Networking;
using Newtonsoft.Json;

namespace BingoAPI.Clients.Implementations;

/// <summary>
/// Default implementation of <see cref="IBingoApiClient"/> for BingoSync
/// </summary>
public class BingoSyncApiClient : IBingoApiClient
{
	private readonly HttpClient _client;

	/// <summary>
	/// Initializes a new instance of the <see cref="BingoSyncApiClient"/> class.
	/// </summary>
	public BingoSyncApiClient(HttpClient client)
	{
		_client = client;
	}

	/// <inheritdoc />
	public Team DefaultTeam => Team.Red;

	#region Helpers

	/// <summary>
	/// Sends the given <see cref="HttpRequestMessage"/>
	/// </summary>
	private Task<HttpResponseMessage> Send(HttpRequestMessage request, CancellationToken ct) =>
		_client.SendAsync(request, ct);

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
			throw new InvalidOperationException(
				$"Failed to deserialize response to {typeof(T).Name}"
			);

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

		using var request = new RequestBuilder().Get().ToEndpoint("").Build();

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

	/// <inheritdoc />
	public async Task<string> CreateRoom(CreateRoomSettings settings, CancellationToken ct)
	{
		var tokens = await GetTokens(ct);

		var body = new Request
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

		using var request = new RequestBuilder().Post().ToEndpoint("/").WithForm(body).Build();

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

	/// <inheritdoc />
	public async Task<string> JoinRoom(JoinRoomSettings settings, CancellationToken ct)
	{
		var body = new DTOs.JoinRoom.Request
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

		var response = await SendAndParse<DTOs.JoinRoom.Response>(request, ct);

		return response.SocketKey;
	}

	/// <inheritdoc />
	public async Task MarkSquare(string room, Team team, int index, CancellationToken ct)
	{
		var body = new DTOs.MarkSquare.Request
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

	/// <inheritdoc />
	public async Task ClearSquare(string room, Team team, int index, CancellationToken ct)
	{
		var body = new DTOs.ClearSquare.Request
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

	/// <inheritdoc />
	public async Task SendMessage(string room, string message, CancellationToken ct)
	{
		var body = new DTOs.SendMessage.Request { Code = room, Message = message };

		using var request = new RequestBuilder()
			.Put()
			.ToEndpoint("/api/chat")
			.WithJson(body)
			.Build();

		await SendAsync(request, ct);
	}

	/// <inheritdoc />
	public async Task ChangeTeam(string room, Team team, CancellationToken ct)
	{
		var body = new DTOs.ChangeTeam.Request { Code = room, Team = team };

		using var request = new RequestBuilder()
			.Put()
			.ToEndpoint("/api/color")
			.WithJson(body)
			.Build();

		await SendAsync(request, ct);
	}

	/// <inheritdoc />
	public async Task<ICollection<Square>> GetSquares(string room, CancellationToken ct)
	{
		using var request = new RequestBuilder().Get().ToEndpoint($"/room/{room}/board").Build();

		return await SendAndParse<Square[]>(request, ct);
	}

	/// <inheritdoc />
	public async Task RevealCard(string room, CancellationToken ct)
	{
		var body = new DTOs.RevealCard.Request { Code = room };

		using var request = new RequestBuilder()
			.Put()
			.ToEndpoint("/api/revealed")
			.WithJson(body)
			.Build();

		await SendAsync(request, ct);
	}

	/// <inheritdoc />
	public async Task<DTOs.GetSocketInformation.Response> GetSocketInformation(
		string socketKey,
		CancellationToken ct
	)
	{
		using var request = new RequestBuilder()
			.Get()
			.ToEndpoint($"/api/socket/{socketKey}")
			.Build();

		return await SendAndParse<DTOs.GetSocketInformation.Response>(request, ct);
	}

	#endregion
}
