using BingoAPI.Events;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace BingoAPI.Networking.Converters;

/// <summary>
/// Converts <see cref="IBingoEvent"/> to and from a <see cref="string"/>
/// </summary>
internal class EventConverter : JsonConverter<IBingoEvent>
{
	/// <inheritdoc />
	public override bool CanWrite => false;

	/// <inheritdoc />
	public override void WriteJson(JsonWriter writer, IBingoEvent? value, JsonSerializer serializer)
	{
		throw new NotImplementedException();
	}

	/// <inheritdoc />
	public override IBingoEvent ReadJson(
		JsonReader reader,
		Type objectType,
		IBingoEvent? existingValue,
		bool hasExistingValue,
		JsonSerializer serializer
	)
	{
		var obj = JObject.Load(reader);

		var type = obj.Value<string>("type");

		IBingoEvent evt = type switch
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
