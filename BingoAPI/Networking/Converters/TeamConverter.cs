using BingoAPI.Models;
using Newtonsoft.Json;

namespace BingoAPI.Networking.Converters;

/// <summary>
/// Converts <see cref="Team"/> to and from a <see cref="string"/>
/// </summary>
internal class TeamConverter : JsonConverter<Team>
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
		["blank"] = Team.None,
	};

	/// <inheritdoc />
	public override void WriteJson(JsonWriter writer, Team value, JsonSerializer serializer)
	{
		var colors = new List<string>();

		foreach (var pair in TeamMappings)
		{
			if (pair.Value == Team.None)
				continue;

			if (!value.HasFlag(pair.Value))
				continue;

			colors.Add(pair.Key);
		}

		writer.WriteValue(string.Join(" ", colors));
	}

	/// <inheritdoc />
	public override Team ReadJson(
		JsonReader reader,
		Type objectType,
		Team existingValue,
		bool hasExistingValue,
		JsonSerializer serializer
	)
	{
		if (reader.Value is not string rawTeam)
			throw new JsonException($"Expected a '{typeof(string)}', but got '{reader.ValueType}'.");

		var result = Team.None;

		foreach (var part in rawTeam.Split([' '], StringSplitOptions.RemoveEmptyEntries))
		{
			if (!TeamMappings.TryGetValue(part, out var team))
				throw new InvalidOperationException($"Unknown team '{part}'");

			result |= team;
		}

		return result;
	}
}
