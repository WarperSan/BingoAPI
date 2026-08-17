using BingoAPI.Configuration.Settings;
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

	/// <summary>
	/// Initializes a new instance of the <see cref="BingoSyncApiClient"/> class.
	/// </summary>
	public BingoSyncApiClient(HttpClient client, JsonSerializerSettings settings)
	{
		_client = new HttpApiClient(client, settings);
	}

	/// <inheritdoc />
	public Task<string> CreateRoom(CreateRoomSettings settings, CancellationToken ct)
	{
		throw new NotImplementedException();
	}

	/// <inheritdoc />
	public async Task<string> JoinRoom(JoinRoomSettings settings, CancellationToken ct)
	{
		var body = new DTOs.BingoSync.JoinRoom.Request
		{
			Code = settings.Code,
			Password = settings.Password,
			Username = settings.Nickname,
			// TODO: Make IsSpectator this a parameter
			IsSpectator = false,
		};

		using var request = new RequestBuilder()
			.Post()
			.ToEndpoint("/api/join-room")
			.WithJson(body)
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
			.WithJson(body)
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
			.WithJson(body)
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
			.WithJson(body)
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
			.WithJson(body)
			.Build();

		var response = await _client.SendRequest(request, ct);

		response.EnsureSuccessStatusCode();
	}

	/// <inheritdoc />
	public Task<ICollection<Square>> GetSquares(string room, CancellationToken ct)
	{
		throw new NotImplementedException();
	}

	/// <inheritdoc />
	public async Task RevealCard(string room, CancellationToken ct)
	{
		var payload = new DTOs.BingoSync.RevealCard.Request { Code = room };

		using var request = new RequestBuilder()
			.Put()
			.ToEndpoint("/api/revealed")
			.WithJson(payload)
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
