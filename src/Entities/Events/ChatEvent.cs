using Newtonsoft.Json.Linq;

namespace BingoAPI.Entities.Events;

/// <summary>
/// Event used when a player sends a message into chat
/// </summary>
public sealed class ChatEvent : BaseEvent
{
	/// <summary>
	/// Content of the message sent
	/// </summary>
	public readonly string Text;

	internal ChatEvent(JObject json) : base(json)
	{
		Text = json.Value<string>("text") ?? "";
	}
}
