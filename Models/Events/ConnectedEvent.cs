using Newtonsoft.Json.Linq;

namespace BingoAPI.Models.Events;

/// <summary>
/// Event that represents someone joining the room
/// </summary>
public class ConnectedEvent : Event
{
    public readonly string RoomId;
    
    public ConnectedEvent(JObject json) : base(json)
    {
        RoomId = json.Value<string>("room") ?? "";
    }
}