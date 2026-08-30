using JetBrains.Annotations;
using Newtonsoft.Json.Linq;

namespace BingoAPI.Events;

/// <summary>
/// Interface that represents any class that can provide <see cref="IEvent"/>
/// </summary>
[PublicAPI]
public interface IEventProvider
{
	/// <summary>
	/// Creates a <see cref="IEvent"/> from the given input
	/// </summary>
	public IEvent Create(string content);
}
