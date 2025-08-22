using System;
using System.Collections.Generic;

namespace BingoAPI.Extensions;

/// <summary>
/// Provides extension methods for <see cref="Dictionary{TKey,TValue}"/>
/// </summary>
public static class DictionaryExtensions
{
    /// <summary>
    /// Gets the value at the given key
    /// </summary>
    /// <remarks>
    /// If the value is not found or is the wrong type, it will default to <see cref="defaultValue"/>
    /// </remarks>
    public static T GetValueOrDefault<T>(this Dictionary<string, object> dictionary, string key, T defaultValue) 
    {
        if (!dictionary.TryGetValue(key, out var value))
            return defaultValue;
        
        if (value is T typedValue)
            return typedValue;
        
        try {
            return (T)Convert.ChangeType(value, typeof(T));
        } 
        catch (InvalidCastException) {
        }

        return defaultValue;
    }
}