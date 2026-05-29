using BingoAPI.Conditions.BuiltIn;
using Newtonsoft.Json.Linq;

namespace BingoAPI.Conditions;

/// <summary>
/// Registry of all known condition factories, keyed by their action
/// </summary>
public static class ConditionRegistry
{
	private static readonly Dictionary<string, Func<ConditionData, ICondition>> Factories = new(StringComparer.OrdinalIgnoreCase);

	static ConditionRegistry()
	{
		// TODO: Register these conditions outside of this class
		Register("AND", AndCondition.Create);
		Register("OR", OrCondition.Create);
		Register("NOT", NotCondition.Create);
		Register("SOME", SomeCondition.Create);
	}

	/// <summary>
	/// Tries to add the given factory under the given action key
	/// </summary>
	/// <returns>Success of the attempt</returns>
	public static bool TryAdd(string action, Func<ConditionData, ICondition> factory)
	{
		if (Factories.ContainsKey(action))
			return false;

		Factories.Add(action, factory);
		return true;
	}

	/// <summary>
	/// Creates a <see cref="ICondition"/> registered under the given action
	/// </summary>
	internal static ICondition Create(string action, ConditionData data)
	{
		if (!Factories.TryGetValue(action, out var factory))
			throw new InvalidOperationException($"No condition registered under the action '{action}'.");

		return factory.Invoke(data);
	}
}
