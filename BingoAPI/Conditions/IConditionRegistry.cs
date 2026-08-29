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
	/// Adds the type <typeparamref name="T"/> under the given key
	/// </summary>
	public void Add<T>(string key)
		where T : ICondition;

	/// <summary>
	/// Adds the given <see cref="ICondition"/> type under the given key
	/// </summary>
	public void Add(Type type, string key);

	/// <summary>
	/// Attempts to get the type added under the given key
	/// </summary>
	public bool TryGet(string key, [NotNullWhen(true)] out ICondition? condition);
}
