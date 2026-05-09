using BingoAPI.Conditions;
using BingoAPI.Conditions;
using BingoAPI.Helpers;
using Newtonsoft.Json.Linq;

namespace BingoAPI.Goals;

/// <summary>
/// Registry of all known condition factories, keyed by their action
/// </summary>
public static class ConditionRegistry
{
	private static readonly Dictionary<string, Func<ConditionData, ICondition>> Factories = new(StringComparer.OrdinalIgnoreCase);

	static ConditionRegistry()
	{
		Register("AND",
			data =>
			{
				var children = data.GetChildren();

				return new AndCondition(children);
			});

		Register("OR",
			data =>
			{
				var children = data.GetChildren();

				return new OrCondition(children);
			});

		Register("NOT",
			data =>
			{
				var child = data.GetChild();

				return new NotCondition(child);
			});

		Register("SOME",
			data =>
			{
				var children = data.GetChildren();
				var amount = data.GetOptionalParam<uint>("amount", 1);

				return new SomeCondition(amount, children);
			});
	}

	/// <summary>
	/// Registers a condition factory under the given action key
	/// </summary>
	public static void Register(string action, Func<ConditionData, ICondition> factory)
	{
		if (!Factories.TryAdd(action, factory))
			throw new InvalidOperationException($"A condition is already registered under '{action}'.");
	}

	/// <summary>
	/// Creates a condition for the given action key using the given JSON
	/// </summary>
	internal static ICondition? Create(string action, JObject json)
	{
		// ReSharper disable once ConvertIfStatementToReturnStatement
		if (!Factories.TryGetValue(action, out var factory))
			return null;

		var data = new ConditionData(json);

		return factory?.Invoke(data);
	}
}
