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
	public override IBingoEvent? ReadJson(
		JsonReader reader,
		Type objectType,
		IBingoEvent? existingValue,
		bool hasExistingValue,
		JsonSerializer serializer
	)
	{
		var obj = JObject.Load(reader);

		var type = obj.Value<string>("type");

		switch (type)
		{
			case "chat":
				return obj.ToObject<ChatEvent>();
			case "goal":
				return obj.ToObject<GoalEvent>();
			case "color":
				return obj.ToObject<ColorEvent>();
			case "connection":
				var evt = obj.ToObject<ConnectionEvent>();

				if (evt == null)
					return null;

				var connectionType = obj.Value<string>("event_type");

				evt.IsConnected = connectionType switch
				{
					"connected" => true,
					"disconnected" => false,
					_ => throw new JsonException($"Unknown 'event_type': '{connectionType}'")
				};

				return evt;
		}

		throw new InvalidOperationException($"No event was found of type '{type}': {obj}");
	}
}
