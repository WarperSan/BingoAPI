using BingoAPI.Models;
using JetBrains.Annotations;

namespace BingoAPI.Events;

/// <summary>
/// Interface that represents any class that can handle <see cref="Event"/>
/// </summary>
[PublicAPI]
public interface IEventHandler
{
	/// <summary>
	/// Handles the given <see cref="Event"/>
	/// </summary>
	public void Handle(Event evt);

	/// <summary>
	/// Notifies that the local player has connected under the given identifier
	/// </summary>
	public void HandleConnect(Player player);

	/// <summary>
	/// Notifies that the local player has disconnected
	/// </summary>
	public void HandleDisconnect();
}
