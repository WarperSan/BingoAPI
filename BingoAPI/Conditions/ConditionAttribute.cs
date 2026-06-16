using System.Reflection;
using BingoAPI.Helpers;

namespace BingoAPI.Conditions;

/// <summary>
/// Defines any method that can make a <see cref="ICondition"/> from a given <see cref="ConditionData"/>
/// </summary>
[AttributeUsage(AttributeTargets.Method | AttributeTargets.Constructor)]
public class ConditionAttribute : Attribute
{
	/// <summary>
	/// Action key to add this factory under
	/// </summary>
	public readonly string Action;

	/// <summary>
	/// Defines this method as a valid factory for <see cref="ICondition"/>
	/// </summary>
	/// <param name="action">
	///	<inheritdoc cref="Action"/>
	/// </param>
	public ConditionAttribute(string action)
	{
		Action = action;
	}

	internal record FactoryEntry
	{
		public required string Action { get; init; }
		public required Func<ConditionData, ICondition> Factory { get; init; }
		public required string SourceName { get; init; }
	}

	/// <summary>
	/// Gets all the factories from the given type
	/// </summary>
	internal static IEnumerable<FactoryEntry> GetFactoriesFromType(Type type)
	{
		foreach (var method in GetFactoryMethods(type))
			yield return method;

		foreach (var ctor in GetFactoryConstructors(type))
			yield return ctor;
	}

	/// <summary>
	/// Gets any method from the given type with the attribute <see cref="ConditionAttribute"/>
	/// </summary>
	private static IEnumerable<FactoryEntry> GetFactoryMethods(Type type)
	{
		var methods = type.GetMethods(
			BindingFlags.Public |
			BindingFlags.NonPublic |
			BindingFlags.Static
		);

		foreach (var method in methods)
		{
			if (method.IsGenericMethodDefinition)
				continue;

			var attribute = method.GetCustomAttribute<ConditionAttribute>();

			if (attribute == null)
				continue;

			if (!typeof(ICondition).IsAssignableFrom(method.ReturnType))
			{
				Log.Warning($"Method '{method.Name}' must return '{nameof(ICondition)}'.");
				continue;
			}

			var parameters = method.GetParameters();

			if (parameters.Length != 1 || parameters[0].ParameterType != typeof(ConditionData))
			{
				Log.Warning($"Method '{method.Name}' must only take '{nameof(ConditionData)}'.");
				continue;
			}

			yield return new FactoryEntry
			{
				Action = attribute.Action,
				Factory = data => (ICondition)method.Invoke(null, [data]),
				SourceName = method.Name,
			};
		}
	}

	/// <summary>
	/// Gets any constructor from the given type with the attribute <see cref="ConditionAttribute"/>
	/// </summary>
	private static IEnumerable<FactoryEntry> GetFactoryConstructors(Type type)
	{
		var constructors = type.GetConstructors(
			BindingFlags.Public |
			BindingFlags.NonPublic |
			BindingFlags.Instance
		);

		foreach (var ctor in constructors)
		{
			var attribute = ctor.GetCustomAttribute<ConditionAttribute>();

			if (attribute == null)
				continue;

			var parameters = ctor.GetParameters();

			if (parameters.Length != 1 || parameters[0].ParameterType != typeof(ConditionData))
			{
				Log.Warning($"Constructor '{type.Name}' must only take '{nameof(ConditionData)}'.");
				continue;
			}

			if (!typeof(ICondition).IsAssignableFrom(type))
				continue;

			yield return new FactoryEntry
			{
				Action = attribute.Action,
				Factory = data => (ICondition)ctor.Invoke([data]),
				SourceName = type.FullName + ctor.Name,
			};
		}
	}
}
