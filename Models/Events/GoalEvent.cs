using Newtonsoft.Json.Linq;

namespace BingoAPI.Models.Events;

/// <summary>
/// Event that represents a goal's state being changed
/// </summary>
public sealed class GoalEvent : Event
{
    public readonly SquareData Square;
    public readonly bool Remove;
    
    public GoalEvent(JObject json) : base(json)
    {
        var goal = json.GetValue("square");
        Square = SquareData.ParseJSON(goal);
        Remove = goal?.Value<bool>("remove") ?? false;
    }
}