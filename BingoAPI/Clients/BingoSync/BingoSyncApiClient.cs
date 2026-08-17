using System.Text.RegularExpressions;
using BingoAPI.Configuration.Settings;
using BingoAPI.Extensions;
using BingoAPI.Models;
using BingoAPI.Network;
using BingoAPI.Networking;
using Newtonsoft.Json;

namespace BingoAPI.Clients.BingoSync;

/// <summary>
/// Default implementation of <see cref="IBingoApiClient"/> for BingoSync
/// </summary>
public class BingoSyncApiClient : IBingoApiClient
{
	private readonly HttpApiClient _client;
	private readonly JsonSerializerSettings _serializerSettings;

	/// <summary>
	/// Initializes a new instance of the <see cref="BingoSyncApiClient"/> class.
	/// </summary>
	public BingoSyncApiClient(HttpClient client, JsonSerializerSettings serializerSettings)
	{
		_client = new HttpApiClient(client, serializerSettings);
		_serializerSettings = serializerSettings;
	}

	/// <summary>
	/// Gets the necessary tokens
	/// </summary>
	private async Task<DTOs.BingoSync.CreateRoom.Tokens> GetTokens(CancellationToken ct)
	{
		// ReSharper disable StringLiteralTypo
		const string PUBLIC_TOKEN = "csrftoken";
		const string CREATION_TOKEN = "csrfmiddlewaretoken";
		// ReSharper restore StringLiteralTypo

		using var request = new RequestBuilder().Get().ToEndpoint("").Build();

		using var response = await _client.SendRequest(request, ct);

		response.EnsureSuccessStatusCode();

		var publicToken = response.GetCookieOrDefault(PUBLIC_TOKEN);

		if (publicToken == null)
			throw new KeyNotFoundException($"No cookie was set for '{PUBLIC_TOKEN}'.");

		var content = await response.Content.ReadAsStringAsync();

		var match = Regex.Match(
			content,
			$"<input[^>]*name=\"{CREATION_TOKEN}\"[^>]*value=\"(.*?)\"[^>]*>"
		);

		if (!match.Success)
			throw new KeyNotFoundException($"Could not find any input with '{CREATION_TOKEN}'.");

		return new DTOs.BingoSync.CreateRoom.Tokens
		{
			PublicToken = publicToken,
			CreationToken = match.Groups[1].Value,
		};
	}

	/// <inheritdoc />
	public async Task<string> CreateRoom(CreateRoomSettings settings, CancellationToken ct)
	{
		var tokens = await GetTokens(ct);

		var body = new DTOs.BingoSync.CreateRoom.Request
		{
			RoomName = settings.Name,
			Password = settings.Password,
			Nickname = nameof(BingoAPI),
			IsLockout = settings.IsLockout,
			Seed = settings.Seed,
			IsRandomized = settings.IsRandomized,
			IsSpectator = settings.IsSpectator,
			HideCard = settings.HideCard,
			// TODO: Add a proper setter
			Board = "",
			CreationToken = tokens.CreationToken,
		};

		using var request = new RequestBuilder().Post().ToEndpoint("/").WithForm(body).Build();

		// ReSharper disable StringLiteralTypo
		request.Headers.Add("Cookie", $"csrftoken={tokens.PublicToken}");
		request.Headers.Add("X-CSRFToken", tokens.CreationToken);
		// ReSharper restore StringLiteralTypo

		using var response = await _client.SendRequest(request, ct);
		response.EnsureSuccessStatusCode();

		if (!request.RequestUri.TryGetRoomCode(out var code))
			throw new KeyNotFoundException(
				$"Could not find room code from '{request.RequestUri}'."
			);

		return code;
	}

	/// <inheritdoc />
	public async Task<string> JoinRoom(JoinRoomSettings settings, CancellationToken ct)
	{
		var body = new DTOs.BingoSync.JoinRoom.Request
		{
			Code = settings.Code,
			Password = settings.Password,
			Username = settings.Nickname,
			IsSpectator = settings.IsSpectator,
		};

		using var request = new RequestBuilder()
			.Post()
			.ToEndpoint("/api/join-room")
			.WithJson(body, _serializerSettings)
			.Build();

		var response = await _client.SendRequest<DTOs.BingoSync.JoinRoom.Response>(request, ct);

		response.EnsureSuccess(out var data);

		return data.SocketKey;
	}

	/// <inheritdoc />
	public async Task MarkSquare(string room, Team team, int index, CancellationToken ct)
	{
		var body = new DTOs.BingoSync.MarkSquare.Request
		{
			Code = room,
			Team = team,
			Index = (index + 1).ToString(),
		};

		using var request = new RequestBuilder()
			.Put()
			.ToEndpoint("/api/select")
			.WithJson(body, _serializerSettings)
			.Build();

		var response = await _client.SendRequest(request, ct);

		response.EnsureSuccessStatusCode();
	}

	/// <inheritdoc />
	public async Task ClearSquare(string room, Team team, int index, CancellationToken ct)
	{
		var body = new DTOs.BingoSync.ClearSquare.Request
		{
			Code = room,
			Team = team,
			Index = (index + 1).ToString(),
		};

		using var request = new RequestBuilder()
			.Put()
			.ToEndpoint("/api/select")
			.WithJson(body, _serializerSettings)
			.Build();

		var response = await _client.SendRequest(request, ct);

		response.EnsureSuccessStatusCode();
	}

	/// <inheritdoc />
	public async Task SendMessage(string room, string message, CancellationToken ct)
	{
		var body = new DTOs.BingoSync.SendMessage.Request { Code = room, Message = message };

		using var request = new RequestBuilder()
			.Put()
			.ToEndpoint("/api/chat")
			.WithJson(body, _serializerSettings)
			.Build();

		var response = await _client.SendRequest(request, ct);

		response.EnsureSuccessStatusCode();
	}

	/// <inheritdoc />
	public async Task ChangeTeam(string room, Team team, CancellationToken ct)
	{
		var body = new DTOs.BingoSync.ChangeTeam.Request { Code = room, Team = team };

		using var request = new RequestBuilder()
			.Put()
			.ToEndpoint("/api/color")
			.WithJson(body, _serializerSettings)
			.Build();

		var response = await _client.SendRequest(request, ct);

		response.EnsureSuccessStatusCode();
	}

	/// <inheritdoc />
	public async Task<ICollection<Square>> GetSquares(string room, CancellationToken ct)
	{
		using var request = new RequestBuilder().Get().ToEndpoint($"/room/{room}/board").Build();

		var response = await _client.SendRequest<DTOs.BingoSync.GetSquares.Square[]>(request, ct);

		response.EnsureSuccess(out var data);

		return
		[
			.. data.Select(s => new Square
			{
				Index = s.Slot.Index,
				Teams = s.Teams,
				Text = s.Text,
			}),
		];
	}

	/// <inheritdoc />
	public async Task RevealCard(string room, CancellationToken ct)
	{
		var body = new DTOs.BingoSync.RevealCard.Request { Code = room };

		using var request = new RequestBuilder()
			.Put()
			.ToEndpoint("/api/revealed")
			.WithJson(body, _serializerSettings)
			.Build();

		var response = await _client.SendRequest(request, ct);

		response.EnsureSuccessStatusCode();
	}

	/// <inheritdoc />
	public async Task<SocketIdentity> GetSocketIdentity(string socketKey, CancellationToken ct)
	{
		using var request = new RequestBuilder()
			.Get()
			.ToEndpoint($"/api/socket/{socketKey}")
			.Build();

		var response = await _client.SendRequest<DTOs.BingoSync.GetSocketIdentity.Response>(
			request,
			ct
		);

		response.EnsureSuccess(out var data);

		return new SocketIdentity { Code = data.Code, PlayerUUID = data.PlayerUUID };
	}
}
