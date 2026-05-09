using BingoAPI.Models;

namespace BingoAPI.Extensions;

/// <summary>
/// Provides extension methods for <see cref="Team"/>
/// </summary>
internal static class TeamExtensions
{
	/// <summary>
	/// Fetches the teams with the given name
	/// </summary>
	public static Team GetTeams(this string name)
	{
		if (string.IsNullOrWhiteSpace(name))
			return Team.None;

		var result = Team.None;

		foreach (var part in name.Split(' '))
		{
			var partTeam = part.ToLowerInvariant() switch
			{
				"pink" => Team.Pink,
				"red" => Team.Red,
				"orange" => Team.Orange,
				"brown" => Team.Brown,
				"yellow" => Team.Yellow,
				"green" => Team.Green,
				"teal" => Team.Teal,
				"blue" => Team.Blue,
				"navy" => Team.Navy,
				"purple" => Team.Purple,
				"blank" => Team.None,
				_ => throw new InvalidOperationException($"Unknown team '{part}'")
			};

			result |= partTeam;
		}

		return result;
	}
}
