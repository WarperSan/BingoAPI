using BingoAPI.Extensions;
using Newtonsoft.Json.Linq;

namespace BingoAPI.Models;

/// <summary>
/// Data for a player
/// </summary>
public readonly struct PlayerData
{
    /// <summary>
    /// Unique Identifier of this player
    /// </summary>
    // ReSharper disable once InconsistentNaming
    public readonly string? UUID;

    /// <summary>
    /// Display name of this player
    /// </summary>
    public readonly string? Name;

    /// <summary>
    /// Team of this player
    /// </summary>
    public readonly Team Team;

    /// <summary>
    /// Represents if this player is a spectator or an active player
    /// </summary>
    public readonly bool IsSpectator;

    internal PlayerData(JToken? obj)
    {
        UUID = obj?.Value<string>("uuid");
        Name = obj?.Value<string>("name");
        Team = obj?.Value<string>("color").GetTeam() ?? Team.Blank;
        IsSpectator = obj?.Value<bool>("is_spectator") ?? false;
    }
}