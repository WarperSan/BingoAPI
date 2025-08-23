using Newtonsoft.Json.Linq;

namespace BingoAPI.Entities.Events;

/// <summary>
/// Event used when a user changes team
/// </summary>
public sealed class ColorEvent : BaseEvent
{
    internal ColorEvent(JObject json) : base(json)
    {
    }
}