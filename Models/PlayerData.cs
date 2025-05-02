using BingoAPI.Extensions;
using Newtonsoft.Json.Linq;

namespace BingoAPI.Models;

/// <summary>
/// Data for a player
/// </summary>
public struct PlayerData
{
    public string? UUID;
    public string? Name;
    public Team Team;
    public bool IsSpectator;

    public static PlayerData ParseJSON(JToken? obj) => new()
    {
        UUID = obj?.Value<string>("uuid"),
        Name = obj?.Value<string>("name"),
        Team = obj?.Value<string>("color").GetTeam() ?? Team.BLANK,
        IsSpectator = obj?.Value<bool>("is_spectator") ?? false
    };
}