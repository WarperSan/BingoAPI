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

		IBingoEvent evt;

		switch (type)
		{
			case "chat":
				evt = new ChatEvent();
				break;
			case "goal":
				evt = new GoalEvent();
				break;
			case "color":
				evt = new ColorEvent();
				break;
			case "revealed":
				evt = new CardRevealedEvent();
				break;
			case "new-card":
				evt = new CardGeneratedEvent();
				break;
			case "connection":
				evt = new ConnectionEvent();
				break;
			default:
				throw new InvalidOperationException($"No event was found of type '{type}': {obj}");
		}

		serializer.Populate(obj.CreateReader(), evt);
		return evt;
	}
}
