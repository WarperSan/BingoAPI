using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace BingoAPI.Conditions;

/// <summary>
/// Registry of all known condition factories, keyed by their action
/// </summary>
public static class ConditionRegistry
{
	private static readonly Dictionary<string, Func<ConditionData, ICondition>> Factories = new(StringComparer.OrdinalIgnoreCase);

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
	/// Parses the given JSON to the appropriate condition
	/// </summary>
	internal static ICondition ParseCondition(JObject json)
	{
		var action = json.Value<string>("action");

		if (action == null)
			throw new JsonException($"No action was specified for this condition: {json}");

		if (!Factories.TryGetValue(action, out var factory))
			throw new InvalidOperationException($"No condition registered under the action '{action}'.");

		var data = new ConditionData(json);

		var condition = factory?.Invoke(data);

		// ReSharper disable once ConvertIfStatementToReturnStatement
		if (condition == null)
			throw new InvalidOperationException($"Unhandled condition '{action}': {json}");

		return condition;
	}
}
