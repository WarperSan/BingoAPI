using System.Diagnostics.CodeAnalysis;
using System.Reflection;
using BingoAPI.Conditions.Factories;
using BingoAPI.Helpers;
using JetBrains.Annotations;

namespace BingoAPI.Conditions;

/// <summary>
/// Registry of all known condition factories, keyed by their action
/// </summary>
public static class ConditionRegistry
{
	private static readonly Dictionary<string, IConditionFactory> FactoryPerAction = new();

	/// <summary>
	/// Attempts to get the <see cref="IConditionFactory"/> registered under the given <paramref name="action"/>
	/// </summary>
	internal static bool TryGetFactory(
		string action,
		[NotNullWhen(true)] out IConditionFactory? factory
	) => FactoryPerAction.TryGetValue(action, out factory);

	/// <summary>
	/// Registers the given <see cref="IConditionFactory"/> under the given action
	/// </summary>
	[PublicAPI]
	public static void RegisterFactory(string action, IConditionFactory factory)
	{
		if (FactoryPerAction.TryGetValue(action, out var oldFactory))
			Log.Debug(
				$"Overriding the factory for '{action}' from '{oldFactory.GetType()}' to '{factory.GetType()}'."
			);

		FactoryPerAction[action] = factory;
	}

	/// <summary>
	/// Registers a new instance of <typeparamref name="T"/> under the given action
	/// </summary>
	[PublicAPI]
	public static void RegisterFactory<T>(string action)
		where T : IConditionFactory, new()
	{
		RegisterFactory(action, new T());
	}

	/// <summary>
	/// Attempts to register a <see cref="IConditionFactory"/> from the given <paramref name="type"/>
	/// </summary>
	[PublicAPI]
	public static bool TryRegisterFactory(Type type)
	{
		if (type.IsAbstract || type.IsInterface)
			return false;

		var attribute = type.GetCustomAttribute<ConditionFactoryAttribute>();

		if (attribute == null)
			return false;

		if (!typeof(IConditionFactory).IsAssignableFrom(type))
			return false;

		if (Activator.CreateInstance(type) is not IConditionFactory factory)
			throw new InvalidOperationException($"Could not create factory '{type}'.");

		try
		{
			Log.Debug($"Registering the factory '{type}' under '{attribute.Action}'.");
			RegisterFactory(attribute.Action, factory);
		}
		catch (Exception e)
		{
			Log.Error($"Error while registering '{type}' under '{attribute.Action}': {e}");
			return false;
		}

		return true;
	}

	/// <summary>
	/// Registers a new instance of <paramref name="type"/> under the given action
	/// </summary>
	internal static void RegisterCondition(string action, Type type)
	{
		var factory = new ConditionJsonFactory(type);
		RegisterFactory(action, factory);
	}

	/// <summary>
	/// Registers a new instance of <typeparamref name="T"/> under the given action
	/// </summary>
	[PublicAPI]
	public static void RegisterCondition<T>(string action)
		where T : ICondition
	{
		RegisterCondition(action, typeof(T));
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
			return false;

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
		where T : ICondition
	{
		return TryRegisterCondition(typeof(T));
	}

	/// <summary>
	/// Registers all <see cref="ICondition"/> and <see cref="IConditionFactory"/> from the given <paramref name="type"/>
	/// </summary>
	[PublicAPI]
	public static void RegisterAllFromType(Type type)
	{
		TryRegisterCondition(type);
		TryRegisterFactory(type);
	}

	/// <summary>
	/// Registers all <see cref="ICondition"/> and <see cref="IConditionFactory"/> from the given <paramref name="assembly"/>
	/// </summary>
	[PublicAPI]
	public static void RegisterAllFromAssembly(Assembly assembly)
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

			RegisterAllFromType(type);
		}
	}

	/// <summary>
	/// Registers all <see cref="ICondition"/> and <see cref="IConditionFactory"/> from the loaded assemblies
	/// </summary>
	[PublicAPI]
	public static void RegisterAll()
	{
		foreach (var assembly in AppDomain.CurrentDomain.GetAssemblies())
			RegisterAllFromAssembly(assembly);
	}
}
