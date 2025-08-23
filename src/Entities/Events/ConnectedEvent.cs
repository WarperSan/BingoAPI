using Newtonsoft.Json.Linq;

namespace BingoAPI.Entities.Events;

/// <summary>
/// Event used when a user joined the room
/// </summary>
public sealed class ConnectedEvent : BaseEvent
{
    /// <summary>
    /// Identifier of the room joined
    /// </summary>
    public readonly string RoomId;
    
    internal ConnectedEvent(JObject json) : base(json)
    {
        RoomId = json.Value<string>("room") ?? "";
    }
}