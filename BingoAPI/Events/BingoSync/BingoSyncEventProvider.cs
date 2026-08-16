using BingoAPI.Extensions;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace BingoAPI.Events.BingoSync;

/// <summary>
/// Default implementation of <see cref="IEventProvider"/> for BingoSync
/// </summary>
public class BingoSyncEventProvider : IEventProvider
{
	private readonly JsonSerializer _jsonSerializer;

	/// <summary>
	/// Initializes a new instance of the <see cref="BingoSyncEventProvider"/> class.
	/// </summary>
	public BingoSyncEventProvider(JsonSerializerSettings serializerSettings)
	{
		_jsonSerializer = JsonSerializer.Create(serializerSettings);
	}

	/// <summary>
	/// Gets the <see cref="Type"/> associated with the given string type
	/// </summary>
	private static Type GetEventType(string type)
	{
		return type switch
		{
			"chat" => typeof(ChatEvent),
			"goal" => typeof(GoalEvent),
			"color" => typeof(ColorEvent),
			"revealed" => typeof(CardRevealedEvent),
			"new-card" => typeof(CardGeneratedEvent),
			"connection" => typeof(ConnectionEvent),
			_ => throw new ArgumentException(
				$"No event type was found of type '{type}'.",
				nameof(type)
			),
		};
	}

	/// <inheritdoc />
	public IEvent Create(JObject obj)
	{
		var rawType = obj.GetRequired<string>("type");

		// TODO: Add support for JSON properties, as Player won't parse correctly

		var type = GetEventType(rawType);

		var rawEvent = obj.ToObject(type, _jsonSerializer);

		if (rawEvent is not IEvent evt)
			throw new ArgumentException(
				$"Failed to parse the given JSON into a supported '{nameof(IEvent)}'.",
				nameof(obj)
			);

		return evt;
	}
}
