using BingoAPI.Configuration.Settings;
using BingoAPI.Models;

namespace BingoAPI.Clients.BingoSync;

/// <summary>
/// Default implementation of <see cref="IBingoApiClient"/> for BingoSync
/// </summary>
public class BingoSyncApiClient : IBingoApiClient
{
	/// <inheritdoc />
	public Task<string> CreateRoom(CreateRoomSettings settings, CancellationToken ct)
	{
		throw new NotImplementedException();
	}

	/// <inheritdoc />
	public Task<string> JoinRoom(JoinRoomSettings settings, CancellationToken ct)
	{
		throw new NotImplementedException();
	}

	/// <inheritdoc />
	public Task MarkSquare(string room, Team team, int index, CancellationToken ct)
	{
		throw new NotImplementedException();
	}

	/// <inheritdoc />
	public Task ClearSquare(string room, Team team, int index, CancellationToken ct)
	{
		throw new NotImplementedException();
	}

	/// <inheritdoc />
	public Task SendMessage(string room, string message, CancellationToken ct)
	{
		throw new NotImplementedException();
	}

	/// <inheritdoc />
	public Task ChangeTeam(string room, Team team, CancellationToken ct)
	{
		throw new NotImplementedException();
	}

	/// <inheritdoc />
	public Task<ICollection<Square>> GetSquares(string room, CancellationToken ct)
	{
		throw new NotImplementedException();
	}

	/// <inheritdoc />
	public Task RevealCard(string room, CancellationToken ct)
	{
		throw new NotImplementedException();
	}

	/// <inheritdoc />
	public Task<SocketIdentity> GetSocketIdentity(string socketKey, CancellationToken ct)
	{
		throw new NotImplementedException();
	}
}
