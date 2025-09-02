using BingoAPI.Extensions;
using Newtonsoft.Json.Linq;

namespace BingoAPI.Models;

/// <summary>
/// Data for a square
/// </summary>
public readonly struct SquareData
{
    /// <summary>
    /// Text displayed by this square
    /// </summary>
    public readonly string? Name;
    
    /// <summary>
    /// Index of this square
    /// </summary>
    /// <remarks>
    /// This index is 1-based
    /// </remarks>
    public readonly int Index;
    
    /// <summary>
    /// Teams owning this square
    /// </summary>
    public readonly Team[] Teams;

    internal SquareData(JToken? obj)
    {
        var slot = obj?.Value<string>("slot")?.Replace("slot", "");

        Name = obj?.Value<string>("name");
        Index = slot != null && int.TryParse(slot, out var index) ? index : -1;
        Teams = obj?.Value<string>("colors").GetTeams() ?? [];
    }
}