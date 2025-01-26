using Newtonsoft.Json.Linq;

namespace BingoAPI.Events;

public class ChatEvent : Event
{
    public readonly string Text;
    
    internal ChatEvent(JObject json) : base(json)
    {
        Text = json.Value<string>("text") ?? "";
    }
}