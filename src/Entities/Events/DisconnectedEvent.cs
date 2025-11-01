using Newtonsoft.Json.Linq;

namespace BingoAPI.Entities.Events;

/// <summary>
/// Event used when a user leaves the room
/// </summary>
public sealed class DisconnectedEvent : BaseEvent
{
    /// <summary>
    /// Identifier of the room left
    /// </summary>
    public readonly string RoomId;

    internal DisconnectedEvent(JObject json) : base(json)
    {
        RoomId = json.Value<string>("room") ?? "";
    }
}