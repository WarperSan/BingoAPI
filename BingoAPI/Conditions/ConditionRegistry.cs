using BingoAPI.Conditions.BuiltIn;

namespace BingoAPI.Conditions;

/// <summary>
/// Registry of all known condition factories, keyed by their action
/// </summary>
public static class ConditionRegistry
{
	private static readonly Dictionary<string, Func<ConditionData, ICondition>> Factories = new(StringComparer.OrdinalIgnoreCase);

	static ConditionRegistry()
	{
		Register("AND", AndCondition.Create);
		Register("OR", OrCondition.Create);
		Register("NOT", NotCondition.Create);
		Register("SOME", SomeCondition.Create);
	}

	/// <summary>
	/// Registers a condition factory under the given action key
	/// </summary>
	public static void Register(string action, Func<ConditionData, ICondition> factory)
	{
		if (Factories.ContainsKey(action))
			throw new InvalidOperationException($"A condition is already registered under '{action}'.");

		Factories.Add(action, factory);
	}

	/// <summary>
	/// Gets the factory registered under the given action key
	/// </summary>
	internal static Func<ConditionData, ICondition> GetFactory(string action)
	{
		if (!Factories.TryGetValue(action, out var factory))
			throw new InvalidOperationException($"No condition registered under the action '{action}'.");

		return factory;
	}
}
