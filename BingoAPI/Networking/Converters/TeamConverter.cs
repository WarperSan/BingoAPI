using System.Runtime.Serialization;
using BingoAPI.Models;
using Newtonsoft.Json;

namespace BingoAPI.Networking.Converters;

/// <summary>
/// Converts a <see cref="Team"/> to and from a <see cref="string"/>
/// </summary>
internal class TeamConverter : JsonConverter<Team>
{
	private static readonly Lazy<Dictionary<string, Team>> TeamMappings = new(() =>
	{
		var teamMap = new Dictionary<string, Team>();
		var names = Enum.GetNames(typeof(Team));

		foreach (var name in names)
		{
			var field = typeof(Team).GetField(name)!;
			var value = (Team)field.GetValue(null)!;

			var specifiedName = field
				.GetCustomAttributes(typeof(EnumMemberAttribute), false)
				.Cast<EnumMemberAttribute>()
				.Select(a => a.Value)
				.SingleOrDefault();

			teamMap.Add(specifiedName ?? name, value);
		}

		return teamMap;
	});

	/// <inheritdoc />
	public override void WriteJson(JsonWriter writer, Team value, JsonSerializer serializer)
	{
		var colors = new List<string>();

		// ReSharper disable once ForeachCanBeConvertedToQueryUsingAnotherGetEnumerator
		foreach (var pair in TeamMappings.Value)
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

		foreach (var part in rawTeam.Split([' '], StringSplitOptions.RemoveEmptyEntries))
		{
			if (!TeamMappings.Value.TryGetValue(part, out var team))
				throw new InvalidOperationException($"Unknown team '{part}'");

			result |= team;
		}

		return result;
	}
}
