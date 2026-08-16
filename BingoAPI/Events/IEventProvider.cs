using Newtonsoft.Json.Linq;

namespace BingoAPI.Events;

/// <summary>
/// Interface that represents any class that can provide <see cref="IEvent"/>
/// </summary>
public interface IEventProvider
{
	/// <summary>
	/// Creates a <see cref="IEvent"/> from the given <see cref="JObject"/>
	/// </summary>
	public IEvent Create(JObject obj);
}
