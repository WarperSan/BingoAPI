using System;
using System.Collections.Generic;
using BingoAPI.Models;

namespace BingoAPI.Extensions;

/// <summary>
/// Provides extension methods for <see cref="Team"/>
/// </summary>
internal static class TeamExtensions
{
    /// <summary>
    /// Fetches the name of the given team
    /// </summary>
    public static string GetName(this Team team) => team.ToString().ToLower().Replace(",", "");

    /// <summary>
    /// Fetches the team with the given name
    /// </summary>
    public static Team GetTeam(this string? name)
    {
        if (string.IsNullOrEmpty(name))
            return Team.BLANK;

        return Enum.TryParse(name.ToUpper(), out Team _team) ? _team : Team.BLANK;
    }

    /// <summary>
    /// Fetches the teams with the given name
    /// </summary>
    public static Team[] GetTeams(this string? name)
    {
        if (string.IsNullOrEmpty(name))
            return [];

        var teams = new List<Team>();

        foreach (var color in name.Split(" "))
        {
            var _team = color.GetTeam();
            
            if (_team == Team.BLANK)
                continue;
            
            teams.Add(_team);
        }

        return teams.ToArray();
    }
}