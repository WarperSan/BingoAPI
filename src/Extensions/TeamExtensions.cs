using System;
using System.Linq;
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
	    if (name == null)
            return Team.Blank;

        return Enum.TryParse(name.ToUpper(), out Team team) ? team : Team.Blank;
    }

    /// <summary>
    /// Fetches the teams with the given name
    /// </summary>
    public static Team[] GetTeams(this string? name)
    {
	    if (name == null)
		    return [];
	    
        return name.Trim().Split(' ').Select(GetTeam).Where(team => team != Team.Blank).ToArray();
    }
}