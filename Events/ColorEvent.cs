using Newtonsoft.Json.Linq;

namespace BingoAPI.Events;

public class ColorEvent : Event
{
    public ColorEvent(JObject json) : base(json)
    {
    }
}