using BingoAPI.Models;

namespace BingoAPI.Extensions;

/// <summary>
/// Extension methods for <see cref="Team"/>
/// </summary>
public static class TeamExtensions
{
	/// <summary>
	/// Gets every individual <see cref="Team"/> from the given combined <see cref="Team"/>
	/// </summary>
	public static Team[] GetTeams(this Team team)
	{
		var teams = new List<Team>();

		foreach (Team teamItem in Enum.GetValues(typeof(Team)))
		{
			if (!team.HasFlag(teamItem))
				continue;

			teams.Add(teamItem);
		}

		return teams.ToArray();
	}
}
