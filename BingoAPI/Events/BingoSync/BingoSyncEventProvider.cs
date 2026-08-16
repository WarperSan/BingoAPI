using Newtonsoft.Json.Linq;

namespace BingoAPI.Events.BingoSync;

/// <summary>
/// Default implementation of <see cref="IEventProvider"/> for BingoSync
/// </summary>
public class BingoSyncEventProvider : IEventProvider
{
	/// <inheritdoc />
	public IEvent Create(JObject obj)
	{
		var type = obj.Value<string>("type");

		IEvent? evt = type switch
		{
			"chat" => obj.ToObject<ChatEvent>(),
			"goal" => obj.ToObject<GoalEvent>(),
			"color" => obj.ToObject<ColorEvent>(),
			"revealed" => obj.ToObject<CardRevealedEvent>(),
			"new-card" => obj.ToObject<CardGeneratedEvent>(),
			"connection" => obj.ToObject<ConnectionEvent>(),
			_ => throw new InvalidOperationException($"No event was found of type '{type}': {obj}"),
		};

		if (evt == null)
			throw new ArgumentException(
				$"Failed to parse the given JSON into a supported '{nameof(IEvent)}'.",
				nameof(obj)
			);

		return evt;
	}
}
