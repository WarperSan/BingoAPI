using System;
using System.Collections.Generic;
//using BingoAPI.Entities.Conditions.Composites;
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
    /// <remarks>
    /// Propagate the constructor, without changing it. It is necessary for <see cref="AddCondition{T}"/>
    /// </remarks>
    protected BaseCondition(JObject json)
    {
    }
    
    /// <summary>
    /// Checks if this condition is met
    /// </summary>
    public abstract bool Check();

    #region Parsing

    private static readonly List<(string, Func<JObject, BaseCondition>)> parsingFallback = [];

    /// <summary>
    /// Parses the given JSON to the appropriate condition
    /// </summary>
    public static BaseCondition? ParseCondition(JObject json)
    {
        try
        {
            var action = json.Value<string>("action");

            switch (action?.ToUpper())
            {
                //case "AND":
                //    return new AndCondition(json);
                //case "OR":
                //    return new OrCondition(json);
                //case "SOME":
                //    return new SomeCondition(json);
                case "NOT":
                    return new NotCondition(json);
            }

            action = action?.ToLower();

            foreach (var (target, parser) in parsingFallback)
            {
                if (action != target)
                    continue;

                var condition = parser.Invoke(json);
            
                if (condition != null)
                    return condition;
            }
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
    /// Adds a parser for any condition with the given action
    /// </summary>
    public static void AddParser(string action, Func<JObject, BaseCondition> callback) => parsingFallback.Add((action.ToLower(), callback));

    /// <summary>
    /// Adds a condition of the given type for the given action
    /// </summary>
    /// <remarks>
    /// The given type must have a constructor with only <see cref="JObject"/> as the parameter
    /// </remarks>
    public static void AddCondition<T>(string action) where T : BaseCondition
    {
        var constructor = typeof(T).GetConstructor([typeof(JObject)]);

        if (constructor == null)
        {
            Log.Error($"Failed to add '{nameof(T)}', because no constructor is valid.");
            return;
        }

        AddParser(action, json => (T)constructor.Invoke([json]));
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