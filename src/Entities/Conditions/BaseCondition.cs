using System;
using System.Collections.Generic;
using System.Reflection;
using BingoAPI.Entities.Conditions.Composites;
using BingoAPI.Entities.Conditions.Decorators;
using BingoAPI.Helpers;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace BingoAPI.Entities.Conditions;

/// <summary>
/// Class that represents the conditions used for marking goals
/// </summary>
public abstract class BaseCondition
{
    /// <summary>
    /// Constructor of a condition
    /// </summary>
    // ReSharper disable once UnusedParameter.Local
    protected BaseCondition(JObject json)
    {
    }
    
    /// <summary>
    /// Checks if this condition is met
    /// </summary>
    public abstract bool Check();

    #region Parsing

    private static readonly Dictionary<string, Func<JObject, BaseCondition>> loadedConditions = [];

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

                Log.Debug($"Added '{type}' with the action '{conditionAttr.Action}'.");
                loadedConditions.TryAdd(conditionAttr.Action, json => (BaseCondition)constructor.Invoke([json]));
            }
        }
    }
    
    /// <summary>
    /// Parses the given JSON to the appropriate condition
    /// </summary>
    public static BaseCondition? ParseCondition(JObject json)
    {
        try
        {
            var action = json.Value<string>("action");

            if (action == null)
            {
                Log.Error($"No action was specified for this condition: {json}");
                return null;
            }

            switch (action.ToUpper())
            {
                case "AND":
                    return new AndCondition(json);
                case "OR":
                    return new OrCondition(json);
                case "SOME":
                    return new SomeCondition(json);
                case "NOT":
                    return new NotCondition(json);
            }

            if (loadedConditions.TryGetValue(action.ToLower(), out var parser))
                return parser.Invoke(json);
        }
        catch (Exception e)
        {
            Log.Error($"Error while parsing condition ('{e.Message}'): {json}");
            return null;
        }
        
        Log.Error($"Unhandled condition: {json}");
        return null;
    }

    /// <summary>
    /// Parses the 'conditions' field from this object
    /// </summary>
    protected static BaseCondition[] ParseConditions(JObject obj)
    {
        var rawConditions = obj.Value<JArray>("conditions");

        if (rawConditions == null)
            throw new JsonException($"Expected 'conditions': {obj}");

        var conditions = new List<BaseCondition>();

        foreach (var rawCondition in rawConditions)
        {
            if (rawCondition is not JObject child)
                continue;

            var newCondition = ParseCondition(child);
            
            if (newCondition == null)
                continue;
            
            conditions.Add(newCondition);
        }

        return conditions.ToArray();
    }

    /// <summary>
    /// Parses the 'params' field from this object
    /// </summary>
    protected static Dictionary<string, object> ParseParameters(JObject obj)
    {
        var rawParams = obj.Value<JObject>("params");

        if (rawParams == null)
            throw new JsonException($"Expected 'params': {obj}");

        var parameters = new Dictionary<string, object>();

        foreach (var property in rawParams.Properties())
        {
            if (property?.Value is not JValue value)
                continue;
            
            if (value.Value == null)
                continue;

            parameters[property.Name] = value.Value;
        }

        return parameters;
    }

    #endregion
}