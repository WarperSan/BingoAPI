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

	extension(string? name)
	{
		/// <summary>
		/// Fetches the team with the given name
		/// </summary>
		public Team GetTeam()
		{
			if (name == null)
				return Team.Blank;

			name = name.ToLower();

			foreach (var teamName in Enum.GetNames(typeof(Team)))
			{
				if (teamName.ToLower() != name)
					continue;

				return (Team)Enum.Parse(typeof(Team), teamName);
			}

			return Team.Blank;
		}

		/// <summary>
		/// Fetches the teams with the given name
		/// </summary>
		public Team[] GetTeams()
		{
			if (name == null)
				return [];

			return name.Trim().Split(' ').Select(GetTeam).Where(team => team != Team.Blank).ToArray();
		}
	}
}
