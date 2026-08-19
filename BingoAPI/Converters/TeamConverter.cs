using BingoAPI.Models;
using Newtonsoft.Json;

namespace BingoAPI.Converters;

/// <summary>
/// Converts a <see cref="Team"/> from and to a <see cref="string"/> using the given team mapping
/// </summary>
public class TeamConverter : JsonConverter<Team>
{
	private readonly Dictionary<string, Team> _teamMapping;

	/// <summary>
	/// Initializes a new instance of the <see cref="TeamConverter"/> class.
	/// </summary>
	public TeamConverter(Dictionary<string, Team> teamMapping)
	{
		_teamMapping = teamMapping;
	}

	/// <inheritdoc />
	public override void WriteJson(JsonWriter writer, Team value, JsonSerializer serializer)
	{
		var colors = new List<string>();

		// ReSharper disable once ForeachCanBeConvertedToQueryUsingAnotherGetEnumerator
		foreach (var pair in _teamMapping)
		{
			if (pair.Value == Team.None && value != Team.None)
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
			throw new JsonException(
				$"Expected a '{typeof(string)}', but got '{reader.ValueType}'."
			);

		var result = Team.None;
		var parts = rawTeam.Split([' '], StringSplitOptions.RemoveEmptyEntries);

		foreach (var part in parts)
		{
			if (!_teamMapping.TryGetValue(part, out var team))
				throw new InvalidOperationException($"Unknown team '{part}'");

			result |= team;
		}

		return result;
	}
}
