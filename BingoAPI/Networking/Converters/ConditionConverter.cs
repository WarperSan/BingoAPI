using BingoAPI.Conditions;
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
		var obj = JObject.Load(reader);

		var action = obj.Value<string>(ACTION_KEY);

		if (action == null)
			throw new JsonException($"Expected '{ACTION_KEY}' property: {obj}");

		if (!ConditionRegistry.TryGetType(action, out var type))
			throw new InvalidOperationException($"No condition has been registered under '{action}'.");

		if (!obj.TryGetValue(PARAMS_KEY, out var paramsToken))
			throw new JsonException($"Expected '{PARAMS_KEY}' property: {obj}");

		try
		{
			var condition = (ICondition)Activator.CreateInstance(type);

			serializer.Populate(paramsToken.CreateReader(), condition);

			return condition;
		}
		catch (Exception e)
		{
			throw new JsonException("Error while parsing the condition.", e);
		}
	}

	/// <inheritdoc />
	public override bool CanConvert(Type objectType) => objectType == typeof(ICondition);
}
