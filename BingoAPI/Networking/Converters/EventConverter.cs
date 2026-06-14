using BingoAPI.Events;
using BingoAPI.Events.BuiltIn;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace BingoAPI.Networking.Converters;

/// <summary>
/// Converts a <see cref="string"/> to a <see cref="IEvent"/>
/// </summary>
internal class EventConverter : JsonConverter
{
	/// <inheritdoc />
	public override bool CanWrite => false;

	/// <inheritdoc />
	public override void WriteJson(JsonWriter writer, object? value, JsonSerializer serializer)
	{
		throw new InvalidOperationException();
	}

	/// <inheritdoc />
	public override object? ReadJson(
		JsonReader reader,
		Type objectType,
		object? existingValue,
		JsonSerializer serializer
	)
	{
		var obj = JObject.Load(reader);

		var type = obj.Value<string>("type");

		return type switch
		{
			"chat" => obj.ToObject<ChatEvent>(),
			"goal" => obj.ToObject<GoalEvent>(),
			"color" => obj.ToObject<ColorEvent>(),
			"revealed" => obj.ToObject<CardRevealedEvent>(),
			"new-card" => obj.ToObject<CardGeneratedEvent>(),
			"connection" => obj.ToObject<ConnectionEvent>(),
			_ => throw new InvalidOperationException($"No event was found of type '{type}': {obj}")
		};
	}

	/// <inheritdoc />
	public override bool CanConvert(Type objectType) => objectType == typeof(IEvent);
}
