using Newtonsoft.Json.Linq;

namespace BingoAPI.Events;

/// <summary>
/// Event that represents someone leaving the room
/// </summary>
public class DisconnectedEvent : Event
{
    public readonly string RoomId;
    
    public DisconnectedEvent(JObject json) : base(json)
    {
        RoomId = json.Value<string>("room") ?? "";
    }
}