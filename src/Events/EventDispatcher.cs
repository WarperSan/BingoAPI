using System.Net.WebSockets;
using BingoAPI.Helpers;

namespace BingoAPI.Events;

/// <summary>
/// Class that parses raw socket messages to <see cref="BaseEvent"/>
/// </summary>
internal sealed class EventDispatcher
{
	private readonly Action<BaseEvent> _onEvent;

	public EventDispatcher(Action<BaseEvent> onEvent)
	{
		_onEvent = onEvent;
	}

	/// <summary>
	/// Callback used when <see cref="WebSocket"/> receives a message
	/// </summary>
	public void OnMessageReceived(string data)
	{
		Log.Debug($"Event received:\n{data}");

		var @event = BaseEvent.ParseEvent(data);

		if (@event == null)
			return;

		_onEvent.Invoke(@event);
	}
}
