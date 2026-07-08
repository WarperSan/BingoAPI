using BingoAPI.Conditions;
using BingoAPI.Helpers;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace BingoAPI.Networking.Converters;

/// <summary>
/// Converts a <see cref="string"/> to a <see cref="ICondition"/>
/// </summary>
internal class ConditionConverter : JsonConverter
{
	private const string ACTION_KEY = "action";
	private const string PARAMS_KEY = "params";

	/// <inheritdoc />
	public override bool CanWrite => false;

	/// <inheritdoc />
	public override void WriteJson(JsonWriter writer, object? value, JsonSerializer serializer)
	{
		throw new InvalidOperationException();
	}

	/// <inheritdoc />
	public override object ReadJson(
		JsonReader reader,
		Type objectType,
		object? existingValue,
		JsonSerializer serializer
	)
	{
		Log.Info("B: " + objectType.Name);

		var obj = JObject.Load(reader);

		var action = obj.Value<string>(ACTION_KEY);

		if (action == null)
			throw new JsonException($"Expected '{ACTION_KEY}' property: {obj}");

		if (!ConditionRegistry.TryGetType(action, out var type))
			throw new InvalidOperationException($"No condition has been registered under '{action}'.");

		if (!obj.TryGetValue(PARAMS_KEY, out var paramsToken))
			throw new JsonException($"Expected '{PARAMS_KEY}' property: {obj}");

		if (paramsToken.ToObject(type) is not ICondition condition)
			throw new JsonException($"Failed to parse '{type}': {paramsToken}");

		return condition;
	}

	/// <inheritdoc />
	public override bool CanConvert(Type objectType) => objectType == typeof(ICondition);
}
