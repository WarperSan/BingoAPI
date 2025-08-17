using Newtonsoft.Json.Linq;

namespace BingoAPI.Models.Events;

/// <summary>
/// Event used when a user marks or unmarks a square
/// </summary>
public sealed class GoalEvent : BaseEvent
{
    /// <summary>
    /// Square modified by this event
    /// </summary>
    public readonly SquareData Square;
    
    /// <summary>
    /// Defines if the selected square has been cleared or marked
    /// </summary>
    public readonly bool HasBeenCleared;
    
    internal GoalEvent(JObject json) : base(json)
    {
        var goal = json.GetValue("square");
        Square = new SquareData(goal);
        HasBeenCleared = goal?.Value<bool>("remove") ?? false;
    }
}