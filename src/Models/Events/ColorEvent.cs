using Newtonsoft.Json.Linq;

namespace BingoAPI.Models.Events;

/// <summary>
/// Event used when a user changes team
/// </summary>
public sealed class ColorEvent : BaseEvent
{
    public ColorEvent(JObject json) : base(json)
    {
    }
}