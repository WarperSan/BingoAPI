using BingoAPI.Clients;
using BingoAPI.Events;
using JetBrains.Annotations;

namespace BingoAPI.Servers;

/// <summary>
/// Interface that represents any class that holds and serves all relevant information about a bingo server
/// </summary>
[PublicAPI]
public interface IBingoServer
{
	/// <summary>
	/// Instance of <see cref="IEventCallback"/> used by this server
	/// </summary>
	public IEventCallback EventCallback { get; }

	/// <summary>
	/// Instance of <see cref="IBingoSessionClient"/> used by this server
	/// </summary>
	public IBingoSessionClient SessionClient { get; }
}
