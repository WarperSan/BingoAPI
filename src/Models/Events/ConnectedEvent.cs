using Newtonsoft.Json.Linq;

namespace BingoAPI.Models.Events;

/// <summary>
/// Event used when a user joined the room
/// </summary>
public sealed class ConnectedEvent : BaseEvent
{
    /// <summary>
    /// Identifier of the room joined
    /// </summary>
    public readonly string RoomId;
    
    public ConnectedEvent(JObject json) : base(json)
    {
        RoomId = json.Value<string>("room") ?? "";
    }
}