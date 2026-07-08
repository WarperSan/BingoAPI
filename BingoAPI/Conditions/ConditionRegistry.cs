using System.Reflection;
using JetBrains.Annotations;

namespace BingoAPI.Conditions;

/// <summary>
/// Registry of all known condition factories, keyed by their action
/// </summary>
[PublicAPI]
public static class ConditionRegistry
{
	private static readonly Dictionary<string, Func<ConditionData, ICondition>> Factories = new(StringComparer.OrdinalIgnoreCase);

	/// <summary>
	/// Tries to add the given factory under the given action key
	/// </summary>
	public static void TryAdd(string action, Func<ConditionData, ICondition> factory)
	{
		if (Factories.ContainsKey(action))
			return;

		Factories.Add(action, factory);
	}

	/// <summary>
	/// Adds every <see cref="ICondition"/> defined using <see cref="ConditionAttribute"/>
	/// </summary>
	public static void AddAll()
	{
		foreach (var assembly in AppDomain.CurrentDomain.GetAssemblies())
		{
			IEnumerable<Type?> types;

			try
			{
				types = assembly.GetTypes();
			}
			catch (ReflectionTypeLoadException ex)
			{
				types = ex.Types;
			}

			foreach (var type in types)
			{
				if (type == null)
					continue;

				if (!type.IsAssignableFrom(typeof(ICondition)))
					continue;

				var attribute = type.GetCustomAttribute<ConditionAttribute>();

				if (attribute == null)
					continue;

				// TODO: Change register
				//TryAdd(attribute.Action, data => );
			}
		}
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
