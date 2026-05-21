using BingoAPI.Events;
using BingoAPI.Events.BuiltIn;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace BingoAPI.Networking.Converters;

/// <summary>
/// Converts a <see cref="string"/> to a <see cref="IEvent"/>
/// </summary>
internal class EventConverter : JsonConverter<IEvent>
{
	/// <inheritdoc />
	public override bool CanWrite => false;

	/// <inheritdoc />
	public override void WriteJson(JsonWriter writer, IEvent? value, JsonSerializer serializer)
	{
		throw new NotImplementedException();
	}

	/// <inheritdoc />
	public override IEvent ReadJson(
		JsonReader reader,
		Type objectType,
		IEvent? existingValue,
		bool hasExistingValue,
		JsonSerializer serializer
	)
	{
		var obj = JObject.Load(reader);

		var type = obj.Value<string>("type");

		IEvent evt = type switch
		{
			"chat" => new ChatEvent(),
			"goal" => new GoalEvent(),
			"color" => new ColorEvent(),
			"revealed" => new CardRevealedEvent(),
			"new-card" => new CardGeneratedEvent(),
			"connection" => new ConnectionEvent(),
			_ => throw new InvalidOperationException($"No event was found of type '{type}': {obj}")
		};

		serializer.Populate(obj.CreateReader(), evt);
		return evt;
	}
}
