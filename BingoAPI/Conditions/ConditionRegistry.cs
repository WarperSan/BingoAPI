using System.Reflection;
using BingoAPI.Helpers;
using Newtonsoft.Json.Linq;

namespace BingoAPI.Conditions;

/// <summary>
/// Class that handles the registering of <see cref="BaseCondition"/>
/// </summary>
public static class ConditionRegistry
{
	private static readonly Dictionary<string, Func<JObject, BaseCondition>> LoadedConditions = [];

	/// <summary>
	/// Refreshes the registry of <see cref="BaseCondition"/>
	/// </summary>
	public static void Refresh()
	{
		LoadedConditions.Clear();

		foreach (var assembly in AppDomain.CurrentDomain.GetAssemblies())
		{
			foreach (var type in assembly.GetTypes())
			{
				if (!typeof(BaseCondition).IsAssignableFrom(type))
					continue;

				var conditionAttr = type.GetCustomAttribute<ConditionAttribute>();

				if (conditionAttr == null)
					continue;

				TryAdd(type, conditionAttr);
			}
		}
	}

	/// <summary>
	/// Creates the given <see cref="BaseCondition"/> from the given content
	/// </summary>
	public static BaseCondition? Create(string action, JObject json)
	{
		// ReSharper disable once ConvertIfStatementToReturnStatement
		if (!LoadedConditions.TryGetValue(action, out var parser))
			return null;

		return parser.Invoke(json);
	}

	/// <summary>
	/// Tries to add the given <see cref="BaseCondition"/>
	/// </summary>
	private static bool TryAdd(Type type, ConditionAttribute attribute)
	{
		var constructor = type.GetConstructor([typeof(JObject)]);

		if (constructor == null)
		{
			Log.Error($"Failed to add '{type}', because no constructor is valid.");
			return false;
		}

		if (LoadedConditions.ContainsKey(attribute.Action))
		{
			Log.Debug($"Couldn't add '{type.FullName}', because another action uses the action '{attribute.Action}'.");
			return false;
		}

		LoadedConditions.Add(
			attribute.Action,
			json => (BaseCondition)constructor.Invoke([json])
		);
		Log.Debug($"Added '{type.FullName}' with the action '{attribute.Action}'.");
		return true;
	}
}
