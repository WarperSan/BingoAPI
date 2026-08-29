using System.Diagnostics.CodeAnalysis;

namespace BingoAPI.Conditions.BuiltIn;

/// <summary>
/// Implementation of <see cref="IConditionRegistry"/> that associates an action keyword to a <see cref="ICondition"/> type
/// </summary>
public sealed class ActionConditionRegistry : IConditionRegistry
{
	private readonly Dictionary<string, Type> _typePerAction = new();

	/// <inheritdoc />
	public void Register<T>(string key)
		where T : ICondition
	{
		Register(typeof(T), key);
	}

	/// <inheritdoc />
	public void Register(Type type, string key)
	{
		if (!typeof(ICondition).IsAssignableFrom(type))
			throw new ArgumentException(
				$"Type '{type}' must implement '{nameof(ICondition)}' interface"
			);

		if (_typePerAction.ContainsKey(key))
			throw new InvalidOperationException($"Type '{type}' is already registered.");

		_typePerAction.Add(key, type);
	}

	/// <inheritdoc />
	public bool TryGetKey<T>([NotNullWhen(true)] out string? key)
	{
		return TryGetKey(typeof(T), out key);
	}

	/// <inheritdoc />
	public bool TryGetKey(Type type, [NotNullWhen(true)] out string? key)
	{
		key = _typePerAction.FirstOrDefault(p => p.Value == type).Key;
		return key != null;
	}

	/// <inheritdoc />
	public bool TryGetType(string key, [NotNullWhen(true)] out Type? type)
	{
		return _typePerAction.TryGetValue(key, out type);
	}
}
