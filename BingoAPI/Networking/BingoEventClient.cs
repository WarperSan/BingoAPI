using System.Net.WebSockets;
using BingoAPI.Events;
using BingoAPI.Helpers;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace BingoAPI.Networking;

/// <summary>
/// Parses raw socket messages to <see cref="BaseEvent"/>
/// </summary>
internal sealed class BingoEventClient
{
	private readonly Action<BaseEvent> _onEvent;

	public BingoEventClient(Action<BaseEvent> onEvent)
	{
		_onEvent = onEvent;
	}

	/// <summary>
	/// Callback used when <see cref="WebSocket"/> receives a message
	/// </summary>
	public void OnMessageReceived(string message)
	{
		Log.Debug($"Event received:\n{message}");

		var @event = ParseEvent(message);

		if (@event == null)
			return;

		_onEvent.Invoke(@event);
	}

	/// <summary>
	/// Parses the given content to the appropriate <see cref="BaseEvent"/>
	/// </summary>
	private static BaseEvent? ParseEvent(string content)
	{
		JObject json;

		try
		{
			json = JObject.Parse(content);
		}
		catch (JsonException ex)
		{
			Log.Error($"Could not parse event JSON: {ex.Message}\n{content}");
			return null;
		}

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
