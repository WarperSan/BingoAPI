using BingoAPI.Extensions;
using Newtonsoft.Json.Linq;

namespace BingoAPI.Models;

/// <summary>
/// Data for a square
/// </summary>
public struct SquareData
{
    public string? Name;
    public int Index;
    public Team[] Teams;

    public static SquareData ParseJSON(JToken? obj)
    {
        var slot = obj?.Value<string>("slot")?.Replace("slot", "");
        return new SquareData
        {
            Name = obj?.Value<string>("name"),
            Index = slot != null && int.TryParse(slot, out var index) ? index : 0,
            Teams = obj?.Value<string>("colors").GetTeams() ?? []
        };
    }
}