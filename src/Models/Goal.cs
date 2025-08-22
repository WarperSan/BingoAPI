using BingoAPI.Models.Conditions;

namespace BingoAPI.Models;

/// <summary>
/// Data for a goal
/// </summary>
/// <remarks>
/// This is only used when creating a room. Once a client joins a room, the data is contained in <see cref="SquareData"/>
/// </remarks>
public class Goal
{
    /// <summary>
    /// Text to show when displaying this goal
    /// </summary>
    public string Name;

    /// <summary>
    /// Condition to fulfill in order to mark this goal
    /// </summary>
    public BaseCondition Condition;
}