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
	public bool TryGet(string key, [NotNullWhen(true)] out ICondition? condition)
	{
		if (!_typePerAction.TryGetValue(key, out var type))
		{
			condition = null;
			return false;
		}

		throw new NotImplementedException();
	}

	/// <inheritdoc />
	public bool TryGetKey<T>([NotNullWhen(true)] out string? key)
	{
		return TryGetKey(typeof(T), out key);
	}

	/// <inheritdoc />
	public bool TryGetKey(Type type, [NotNullWhen(true)] out string? key)
	{
		foreach (var pair in _typePerAction)
		{
			if (pair.Value != type)
				continue;

			key = pair.Key;
			return true;
		}

		key = null;
		return false;
	}

	/// <inheritdoc />
	public IEnumerable<string> GetConditionParameters(string key)
	{
		if (!_typePerAction.TryGetValue(key, out var type))
			throw new ArgumentException($"No condition has been added under '{key}'.", nameof(key));

		return type.GetProperties().Select(p => p.Name);
	}
}
