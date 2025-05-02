using System;
using System.Collections.Generic;
using BingoAPI.Models;
using UnityEngine;

namespace BingoAPI.Extensions;

public static class BingoTeamExtension
{
    /// <summary>
    /// Fetches the name of the given team
    /// </summary>
    public static string GetName(this Team team) => team.ToString().ToLower();

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

    /// <summary>
    /// Fetches all the teams
    /// </summary>
    public static Team[] GetAllTeams()
    {
        var array = Enum.GetValues(typeof(Team));
        var teams = new List<Team>();

        foreach (Team team in array)
        {
            if (team == Team.BLANK)
                continue;
            
            teams.Add(team);
        }

        return teams.ToArray();
    }

    /// <summary>
    /// Fetches the HEX color of the given team
    /// </summary>
    public static string GetHexColor(this Team team)
    {
        switch (team)
        {
            case Team.PINK:
                return "#ED86AA";
            case Team.RED:
                return "#FF4944";
            case Team.ORANGE:
                return "#FF9C12";
            case Team.BROWN:
                return "#AB5C23";
            case Team.YELLOW:
                return "#D8D014";
            case Team.GREEN:
                return "#31D814";
            case Team.TEAL:
                return "#419695";
            case Team.BLUE:
                return "#409CFF";
            case Team.NAVY:
                return "#0D48B5";
            case Team.PURPLE:
                return "#822DBF";
            case Team.BLANK:
            default:
                return "#FFFFFF";
        }
    }

    /// <summary>
    /// Fetches the color of the given team
    /// </summary>
    public static Color GetColor(this Team team)
    {
        var hex = team.GetHexColor();

        if (ColorUtility.DoTryParseHtmlColor(hex, out var color))
            return color;
        return Color.white;
    }
}