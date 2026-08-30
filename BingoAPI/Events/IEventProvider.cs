using JetBrains.Annotations;

namespace BingoAPI.Events;

/// <summary>
/// Interface that represents any class that can provide <see cref="Event"/>
/// </summary>
[PublicAPI]
public interface IEventProvider
{
	/// <summary>
	/// Creates a <see cref="Event"/> from the given input
	/// </summary>
	public Event Create(string content);
}
