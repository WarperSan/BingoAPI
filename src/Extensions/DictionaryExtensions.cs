namespace BingoAPI.Extensions;

/// <summary>
/// Provides extension methods for <see cref="Dictionary{TKey,TValue}"/>
/// </summary>
public static class DictionaryExtensions
{
    private static bool GetValue<T>(Dictionary<string, object> dictionary, string key, out T? value)
    {
        if (!dictionary.TryGetValue(key, out var rawValue))
        {
            value = default;
            return false;
        }

        if (rawValue is T typedValue)
        {
            value = typedValue;
            return true;
        }

        try
        {
            value = (T?)Convert.ChangeType(rawValue, typeof(T));
            return true;
        }
        catch (InvalidCastException)
        {
        }

        value = default;
        return false;
    }

    /// <summary>
    /// Gets the value at the given key
    /// </summary>
    /// <remarks>
    /// If the value is not found or is the wrong type, it will default to given value
    /// </remarks>
    public static T GetOptionalValue<T>(this Dictionary<string, object> dictionary, string key, T defaultValue)
    {
        if (!GetValue<T>(dictionary, key, out var value))
            return defaultValue;

        return value ?? defaultValue;
    }

    /// <summary>
    ///     Gets the value at the given key
    /// </summary>
    /// <remarks>
    ///     If the value found is of the wrong type or is missing, an exception is raised
    /// </remarks>
    public static T GetRequiredValue<T>(this Dictionary<string, object> dictionary, string key)
    {
        if (GetValue<T>(dictionary, key, out var value) && value != null)
            return value;

        throw new NullReferenceException($"The key '{key}' is required to be of type '{typeof(T)}'.");
    }
}