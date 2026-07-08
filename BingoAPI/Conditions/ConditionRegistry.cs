using System.Diagnostics.CodeAnalysis;
using System.Reflection;
using BingoAPI.Helpers;
using JetBrains.Annotations;

namespace BingoAPI.Conditions;

/// <summary>
/// Registry of all known condition factories, keyed by their action
/// </summary>
[PublicAPI]
public static class ConditionRegistry
{
	private static readonly Dictionary<string, Type> TypePerAction = new(StringComparer.OrdinalIgnoreCase);

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

				if (type.IsAbstract || type.IsInterface)
					continue;

				if (!typeof(ICondition).IsAssignableFrom(type))
					continue;

				var attribute = type.GetCustomAttribute<ConditionAttribute>();

				if (attribute == null)
					continue;

				Log.Debug($"Registering '{type}' under '{attribute.Action}'.");
				TypePerAction[attribute.Action] = type;
			}
		}
	}

	/// <summary>
	/// Attempts to find the type associated with the given action
	/// </summary>
	public static bool TryGetType(string action, [NotNullWhen(true)] out Type? type) => TypePerAction.TryGetValue(action, out type);
}
