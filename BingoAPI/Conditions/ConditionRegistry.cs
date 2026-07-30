using System.Reflection;
using BingoAPI.Conditions.Factories;
using BingoAPI.Helpers;
using JetBrains.Annotations;
using Newtonsoft.Json;

namespace BingoAPI.Conditions;

/// <summary>
/// Registry of all known condition factories, keyed by their action
/// </summary>
public static class ConditionRegistry
{
	private static readonly Dictionary<string, IConditionFactory> FactoryPerAction = new();

	/// <summary>
	/// Adds every <see cref="ICondition"/> defined using <see cref="ConditionAttribute"/>
	/// </summary>
	[PublicAPI]
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

				TryRegisterCondition(type);
			}
		}
	}

	/// <summary>
	/// Attempts to register a <see cref="ICondition"/> from the given <paramref name="type"/>
	/// </summary>
	[PublicAPI]
	public static bool TryRegisterCondition(Type type)
	{
		if (type.IsAbstract || type.IsInterface)
			return false;

		var attribute = type.GetCustomAttribute<ConditionAttribute>();

		if (attribute == null)
			return false;

		if (!typeof(ICondition).IsAssignableFrom(type))
		{
			Log.Warning(
				$"The attribute '{nameof(ConditionAttribute)}' must be used on a class inheriting the interface '{nameof(ICondition)}'."
			);
			return false;
		}

		try
		{
			Log.Debug($"Registering the condition '{type}' under '{attribute.Action}'.");
			RegisterCondition(attribute.Action, type);
		}
		catch (Exception e)
		{
			Log.Error($"Error while registering '{type}' under '{attribute.Action}': {e}");
			return false;
		}

		return true;
	}

	/// <summary>
	/// Attempts to register a <see cref="ICondition"/> from the given <typeparamref name="T"/>
	/// </summary>
	[PublicAPI]
	public static bool TryRegisterCondition<T>()
		where T : ICondition => TryRegisterCondition(typeof(T));

	/// <summary>
	/// Registers the given <see cref="IConditionFactory"/> under the given action
	/// </summary>
	[PublicAPI]
	public static void RegisterFactory(string action, IConditionFactory factory)
	{
		if (FactoryPerAction.ContainsKey(action))
			throw new InvalidOperationException(
				$"A factory has already been registered under '{action}'."
			);

		FactoryPerAction.Add(action, factory);
	}

	/// <summary>
	/// Registers an new instance of <typeparamref name="T"/> under the given action
	/// </summary>
	[PublicAPI]
	public static void RegisterFactory<T>(string action)
		where T : IConditionFactory, new() => RegisterFactory(action, new T());

	/// <summary>
	/// Registers an new instance of <paramref name="type"/> under the given action
	/// </summary>
	internal static void RegisterCondition(string action, Type type)
	{
		var factory = new ConditionJsonFactory(type);
		RegisterFactory(action, factory);
	}

	/// <summary>
	/// Registers an new instance of <typeparamref name="T"/> under the given action
	/// </summary>
	[PublicAPI]
	public static void RegisterCondition<T>(string action)
		where T : ICondition => RegisterCondition(action, typeof(T));

	/// <summary>
	/// Creates an instance of <see cref="ICondition"/> by using the given action and parameters
	/// </summary>
	internal static ICondition CreateFromJson(
		string action,
		JsonReader reader,
		JsonSerializer serializer
	)
	{
		if (!FactoryPerAction.TryGetValue(action, out var factory))
			throw new ArgumentException($"No factory has been registered under '{action}'.");

		return factory.Generate(reader, serializer);
	}
}
