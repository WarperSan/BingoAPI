using Newtonsoft.Json.Linq;

namespace BingoAPI.Models.Events;

/// <summary>
/// Event that represents someone talking in chat
/// </summary>
public class ChatEvent : Event
{
    public readonly string Text;
    
    internal ChatEvent(JObject json) : base(json)
    {
        Text = json.Value<string>("text") ?? "";
    }
}