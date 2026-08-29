using System.Diagnostics.CodeAnalysis;
using JetBrains.Annotations;

namespace BingoAPI.Conditions;

/// <summary>
/// Interface that represents any class that register a <see cref="ICondition"/> type to a unique identifier
/// </summary>
[PublicAPI]
public interface IConditionRegistry
{
	/// <summary>
	/// Registers the type <typeparamref name="T"/> under the given key
	/// </summary>
	public void Register<T>(string key)
		where T : ICondition;

	/// <summary>
	/// Registers the given type under the given key
	/// </summary>
	/// <remarks>
	///	<paramref name="type"/> must implement <see cref="ICondition"/>
	/// </remarks>
	public void Register(Type type, string key);

	/// <summary>
	/// Attempts to get the type added under the given key
	/// </summary>
	public bool TryGet(string key, [NotNullWhen(true)] out ICondition? condition);

	/// <summary>
	/// Attempts to get the key of the given <typeparamref name="T"/>
	/// </summary>
	public bool TryGetKey<T>([NotNullWhen(true)] out string? key);

	/// <summary>
	/// Attempts to get the key of the given <see cref="Type"/>
	/// </summary>
	public bool TryGetKey(Type type, [NotNullWhen(true)] out string? key);

	public IEnumerable<string> GetConditionParameters(string key);
}
