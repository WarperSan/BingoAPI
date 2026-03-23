using BingoAPI.Helpers;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace BingoAPI.Events;

/// <summary>
/// Class that handles the parsing of <see cref="BaseEvent"/>
/// </summary>
public static class EventParser
{
	/// <summary>
	/// Parses the given JSON to the appropriate <see cref="BaseEvent"/>
	/// </summary>
	public static BaseEvent? ParseEvent(string content)
	{
		var json = JsonConvert.DeserializeObject<JObject>(content);

		if (json == null)
		{
			Log.Error($"Could not create a JSON object with the given event: {content}");
			return null;
		}

		return ParseEvent(json);
	}

	/// <summary>
	/// Parses the given JSON to the appropriate <see cref="BaseEvent"/>
	/// </summary>
	public static BaseEvent? ParseEvent(JObject json)
	{
		var type = json.Value<string>("type") ?? "";

		switch (type)
		{
			case "connection":
				var eventType = json.Value<string>("event_type");

				switch (eventType)
				{
					case "connected":
						return new ConnectedEvent(json);
					case "disconnected":
						return new DisconnectedEvent(json);
				}
				break;
			case "chat":
				return new ChatEvent(json);
			case "color":
				return new ColorEvent(json);
			case "goal":
				return new GoalEvent(json);
		}

		Log.Error($"Unhandled event: {json}");
		return null;
	}
}
