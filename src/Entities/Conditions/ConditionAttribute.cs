using System.Reflection;
using BingoAPI.Helpers;
using Newtonsoft.Json.Linq;

namespace BingoAPI.Entities.Conditions;

/// <summary>
///     Attribute that holds information about <see cref="BaseCondition"/>
/// </summary>
[AttributeUsage(AttributeTargets.Class, Inherited = false)]
// ReSharper disable once ClassNeverInstantiated.Global
public sealed class ConditionAttribute : Attribute
{
    /// <summary>
    ///     Name of the action used by this <see cref="BaseCondition"/>
    /// </summary>
    public readonly string Action;

    /// <summary>
    ///     Registers this condition to the given action
    /// </summary>
    /// <remarks>
    ///     The action is case-sensitive
    /// </remarks>
    public ConditionAttribute(string action)
    {
        Action = action;
    }
    
    private static readonly Dictionary<string, Func<JObject, BaseCondition>> LoadedConditions = [];

    /// <summary>
    ///     Loads every <see cref="BaseCondition"/> with the attribute <see cref="ConditionAttribute"/>
    /// </summary>
    internal static void LoadConditions()
    {
        foreach (var assembly in AppDomain.CurrentDomain.GetAssemblies())
        {
            foreach (var type in assembly.GetTypes())
            {
                if (!typeof(BaseCondition).IsAssignableFrom(type))
                    continue;

                var conditionAttr = type.GetCustomAttribute<ConditionAttribute>();

                if (conditionAttr == null)
                    continue;

                var constructor = type.GetConstructor([typeof(JObject)]);

                if (constructor == null)
                {
                    Log.Error($"Failed to add '{type}', because no constructor is valid.");
                    continue;
                }

                var success = false;

                if (LoadedConditions.ContainsKey(conditionAttr.Action))
                {
	                LoadedConditions.Add(
		                conditionAttr.Action,
		                json => (BaseCondition)constructor.Invoke([json])
		            );
	                success = true;
                }
                
                // ReSharper disable once ConvertIfStatementToConditionalTernaryExpression
                if (success)
                    Log.Debug($"Added '{type.FullName}' with the action '{conditionAttr.Action}'.");
                else
                    Log.Debug($"Couldn't add '{type.FullName}', because another action uses the action '{conditionAttr.Action}'.");
            }
        }
    }
    
    /// <summary>
    ///     Gets the <see cref="BaseCondition"/> associated with the given action
    /// </summary>
    internal static BaseCondition? GetCondition(string action, JObject json)
    {
        // ReSharper disable once ConvertIfStatementToReturnStatement
        if (!LoadedConditions.TryGetValue(action, out var parser))
            return null;

        return parser.Invoke(json);
    }
}