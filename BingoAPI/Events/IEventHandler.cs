using BingoAPI.Models;
using JetBrains.Annotations;

namespace BingoAPI.Events;

/// <summary>
/// Interface that represents any class that can handle <see cref="IEvent"/>
/// </summary>
[PublicAPI]
public interface IEventHandler
{
	/// <summary>
	/// Handles the given <see cref="IEvent"/>
	/// </summary>
	public void Handle(IEvent evt);

	/// <summary>
	/// Notifies that the player has connected under the given identifier
	/// </summary>
	public void HandleConnect(Player player);

	/// <summary>
	/// Notifies that the player has disconnected
	/// </summary>
	public void HandleDisconnect();
}
