namespace BingoAPI.Models;

/// <summary>
/// Representation of a goal
/// </summary>
public struct Goal
{
    /// <summary>
    /// Unique ID of this goal
    /// </summary>
    /// <remarks>
    /// If there is another goal with the same value, one of them won't be registered
    /// </remarks>
    public string GUID;

    /// <summary>
    /// Defines if this goal is active or not
    /// </summary>
    public bool IsActive;
    
    /// <summary>
    /// Text to show when displaying this goal
    /// </summary>
    public string Title;
}