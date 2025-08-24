namespace BingoAPI.Models;

/// <summary>
/// Data for a goal
/// </summary>
/// <remarks>
/// This is only used when creating a room. Once a client joins a room, the data is contained in <see cref="SquareData"/>
/// </remarks>
public struct Goal
{
    /// <summary>
    /// Text to show when displaying this goal
    /// </summary>
    public string Name;
}