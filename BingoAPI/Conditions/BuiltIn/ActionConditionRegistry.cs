using System.Diagnostics.CodeAnalysis;

namespace BingoAPI.Conditions.BuiltIn;

/// <summary>
/// Implementation of <see cref="IConditionRegistry"/> that associates an action keyword to a <see cref="ICondition"/> type
/// </summary>
public sealed class ActionConditionRegistry : IConditionRegistry
{
	private readonly Dictionary<string, Type> _typePerAction = new();

	/// <inheritdoc />
	public void Add<T>(string key)
		where T : ICondition
	{
		Add(typeof(T), key);
	}

	/// <inheritdoc />
	public void Add(Type type, string key)
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
}
