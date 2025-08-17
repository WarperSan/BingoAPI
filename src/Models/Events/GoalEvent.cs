using Newtonsoft.Json.Linq;

namespace BingoAPI.Models.Events;

/// <summary>
/// Event used when a user marks or unmarks a square
/// </summary>
public sealed class GoalEvent : BaseEvent
{
    public readonly SquareData Square;
    public readonly bool Remove;
    
    public GoalEvent(JObject json) : base(json)
    {
        var goal = json.GetValue("square");
        Square = new SquareData(goal);
        Remove = goal?.Value<bool>("remove") ?? false;
    }
}