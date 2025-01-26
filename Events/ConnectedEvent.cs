using Newtonsoft.Json.Linq;

namespace BingoAPI.Events;

public class ConnectedEvent : Event
{
    public readonly string RoomId;
    
    public ConnectedEvent(JObject json) : base(json)
    {
        RoomId = json.Value<string>("room") ?? "";
    }
}