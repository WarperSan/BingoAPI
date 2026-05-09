using BingoAPI.Models;

namespace BingoAPI.Extensions;

/// <summary>
/// Provides extension methods for <see cref="Team"/>
/// </summary>
internal static class TeamExtensions
{
	private static readonly Dictionary<string, Team> TeamMappings = new(StringComparer.OrdinalIgnoreCase)
	{
		["pink"] = Team.Pink,
		["red"] = Team.Red,
		["orange"] = Team.Orange,
		["brown"] = Team.Brown,
		["yellow"] = Team.Yellow,
		["green"] = Team.Green,
		["teal"] = Team.Teal,
		["blue"] = Team.Blue,
		["navy"] = Team.Navy,
		["purple"] = Team.Purple,
		["blank"] = Team.None
	};

	/// <summary>
	/// Converts a <see cref="string"/> into <see cref="Team"/>
	/// </summary>
	public static Team FromColorString(this string name)
	{
		if (string.IsNullOrWhiteSpace(name))
			return Team.None;

		var result = Team.None;

		foreach (var part in name.Split(' '))
		{
			if (!TeamMappings.TryGetValue(part, out var team))
				throw new InvalidOperationException($"Unknown team '{part}'");

			result |= team;
		}

		return result;
	}

	/// <summary>
	/// Converts <see cref="Team"/> into a <see cref="string"/>
	/// </summary>
	public static string ToColorString(this Team team)
	{
		var colors = new List<string>();

		foreach (var pair in TeamMappings)
		{
			if (pair.Value == Team.None)
				continue;

			if (!team.HasFlag(pair.Value))
				continue;

			colors.Add(pair.Key);
		}

		return string.Join(" ", colors);
	}
}
