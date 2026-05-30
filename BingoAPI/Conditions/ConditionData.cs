using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace BingoAPI.Conditions;

/// <summary>
/// Wraps the raw data of <see cref="ICondition"/>
/// </summary>
public sealed class ConditionData
{
	private readonly JObject _json;

	internal ConditionData(JObject json)
	{
		_json = json;
	}

	/// <summary>
	/// Tries to get the parameter at the given key
	/// </summary>
	private bool TryGetParameter<T>(string key, out T? value)
	{
		value = default;

		if (_json["params"] is not JObject @params)
			return false;

		if (!@params.TryGetValue(key, out var token))
			return false;

		if (token.Type == JTokenType.Null)
			return false;

		value = token.ToObject<T>();
		return value is not null;
	}

	/// <summary>
	/// Gets the parameter at the given key, or throws an exception if invalid
	/// </summary>
	public T GetRequiredParameter<T>(string key) where T : notnull
	{
		if (!TryGetParameter<T>(key, out var value) || value == null)
			throw new JsonException($"Failed to find '{key}' of type '{typeof(T)}'.");

		return value;
	}

	/// <summary>
	/// Gets the parameter at the given key, or <paramref name="defaultValue"/> if not found
	/// </summary>
	public T GetOptionalParameter<T>(string key, T defaultValue) where T : notnull
	{
		if (!TryGetParameter<T>(key, out var value))
			return defaultValue;

		return value ?? defaultValue;
	}
}
