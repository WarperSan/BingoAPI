using BingoAPI.Models;
using Newtonsoft.Json.Linq;

namespace BingoAPI.Events;

/// <summary>
/// Event that represents a goal's state being changed
/// </summary>
public class GoalEvent : Event
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