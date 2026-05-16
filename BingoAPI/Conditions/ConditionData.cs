using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace BingoAPI.Conditions;

/// <summary>
/// Wraps the raw data of <see cref="ICondition"/>
/// </summary>
public sealed class ConditionData
{
	private const string CONDITIONS_KEY = "conditions";
	private const string CONDITION_KEY = "condition";
	private const string PARAMS_KEY = "params";

	private readonly JObject _json;
	private readonly JObject? _params;

	internal ConditionData(JObject json)
	{
		_json = json;
		_params = json.Value<JObject>(PARAMS_KEY);
	}

	/// <summary>
	/// Gets the children conditions from the <c>conditions</c> field
	/// </summary>
	/// <returns></returns>
	public ICondition[] GetChildren()
	{
		if (_params == null)
			throw new JsonException($"Expected '{PARAMS_KEY}' object: {_json}");

		var rawConditions = _params.Value<JArray>(CONDITIONS_KEY);

		if (rawConditions == null)
			throw new JsonException($"Expected '{CONDITIONS_KEY}': {_params}");

		var conditions = new List<ICondition>();

		foreach (var rawCondition in rawConditions)
		{
			if (rawCondition is not JObject child)
				continue;

			var newCondition = child.ToObject<ICondition>();

			if (newCondition == null)
				throw new InvalidOperationException($"Unhandled condition: {child}");

			conditions.Add(newCondition);
		}

		return conditions.ToArray();
	}

	/// <summary>
	/// Gets the child condition from the <c>condition</c> field
	/// </summary>
	public ICondition GetChild()
	{
		if (_params == null)
			throw new JsonException($"Expected '{PARAMS_KEY}' object: {_json}");

		var rawCondition = _params.Value<JObject>(CONDITION_KEY);

		if (rawCondition == null)
			throw new JsonException($"Expected '{CONDITION_KEY}': {_params}");

		var newCondition = rawCondition.ToObject<ICondition>();

		// ReSharper disable once ConvertIfStatementToReturnStatement
		if (newCondition == null)
			throw new InvalidOperationException($"Unhandled condition: {rawCondition}");

		return newCondition;
	}

	/// <summary>
	/// Gets the required parameter with the given key
	/// </summary>
	public T GetRequiredParam<T>(string key)
	{
		if (_params == null)
			throw new JsonException($"Expected '{PARAMS_KEY}' object: {_json}");

		var value = _params.Value<T>(key);

		// ReSharper disable once ConvertIfStatementToReturnStatement
		if (value == null)
			throw new JsonException($"Expected '{key}' in parameters: {_params}");

		return value;
	}

	/// <summary>
	/// Gets the optional parameter with the given key
	/// </summary>
	public T GetOptionalParam<T>(string key, T defaultValue)
	{
		if (_params == null)
			return defaultValue;

		var value = _params.Value<T>(key);

		return value ?? defaultValue;
	}
}
