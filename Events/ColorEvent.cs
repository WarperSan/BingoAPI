using Newtonsoft.Json.Linq;

namespace BingoAPI.Events;

/// <summary>
/// Event that represents someone changing team
/// </summary>
public class ColorEvent : Event
{
    public ColorEvent(JObject json) : base(json)
    {
    }
}