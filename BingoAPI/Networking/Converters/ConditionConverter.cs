using BingoAPI.Conditions;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace BingoAPI.Networking.Converters;

/// <summary>
/// Converts a <see cref="string"/> to a <see cref="ICondition"/>
/// </summary>
internal class ConditionConverter : JsonConverter<ICondition>
{
	private const string ACTION_KEY = "action";

	/// <inheritdoc />
	public override bool CanWrite => false;

	/// <inheritdoc />
	public override void WriteJson(JsonWriter writer, ICondition? value, JsonSerializer serializer)
	{
		throw new NotImplementedException();
	}

	/// <inheritdoc />
	public override ICondition ReadJson(
		JsonReader reader,
		Type objectType,
		ICondition? existingValue,
		bool hasExistingValue,
		JsonSerializer serializer
	)
	{
		var obj = JObject.Load(reader);

		var action = obj.Value<string>(ACTION_KEY);

		if (action == null)
			throw new JsonException($"Expected '{ACTION_KEY}' property: {obj}");

		return ConditionRegistry.Create(action, obj);
	}
}
