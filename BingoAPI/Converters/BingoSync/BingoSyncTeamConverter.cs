using BingoAPI.Models;

namespace BingoAPI.Converters.BingoSync;

/// <summary>
/// Implementation of <see cref="TeamConverter"/> specific for BingoSync
/// </summary>
public sealed class BingoSyncTeamConverter : TeamConverter
{
	private static readonly Dictionary<string, Team> TeamMapping = new()
	{
		["blank"] = Team.None,
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
	};

	/// <inheritdoc />
	public BingoSyncTeamConverter()
		: base(TeamMapping) { }
}
