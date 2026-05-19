using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace BingoAPI.Conditions;

/// <summary>
/// Wraps the raw data of <see cref="ICondition"/>
/// </summary>
public sealed class ConditionData
{
	private const string PARAMS_KEY = "params";

	private readonly JObject _json;
	private readonly JObject? _params;

	internal ConditionData(JObject json)
	{
		_json = json;
		_params = json.Value<JObject>(PARAMS_KEY);
	}

	/// <summary>
	/// Gets the parameter at the given key
	/// </summary>
	public T GetRequiredParameter<T>(string key) where T : notnull
	{
		if (_params == null)
			throw new JsonException($"Expected '{PARAMS_KEY}', but it was not found in '{_json}'.");

		if (!_params.TryGetValue(key, out var valueToken))
			throw new JsonException($"Expected '{key}', but it was not found in '{_params}'.");

		var value = valueToken.ToObject<T>();

		// ReSharper disable once ConvertIfStatementToReturnStatement
		if (value == null)
			throw new JsonException($"Expected '{key}' to be '{typeof(T)}', got '{valueToken.Type}'.");

		return value;
	}

	/// <summary>
	/// Gets the parameter at the given key, or <paramref name="defaultValue"/> if not found
	/// </summary>
	public T GetOptionalParameter<T>(string key, T defaultValue) where T : notnull
	{
		if (_params == null)
			return defaultValue;

		return _params.Value<T>(key) ?? defaultValue;
	}

	/// <summary>
	/// Gets the conditions defined at the <c>conditions</c> property
	/// </summary>
	public ICondition[] GetChildren() => GetRequiredParameter<ICondition[]>("conditions");

	/// <summary>
	/// Gets the condition defined at the <c>condition</c> property
	/// </summary>
	public ICondition GetChild() => GetRequiredParameter<ICondition>("condition");
}
