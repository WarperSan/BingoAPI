using BingoAPI.Extensions;
using BingoAPI.Models;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace BingoAPI.Converters.BingoSync;

/// <summary>
/// Converts a <see cref="Player"/> from and to a <see cref="string"/>
/// </summary>
public sealed class BingoSyncPlayerConverter : JsonConverter<Player>
{
	private const string UUID_PROPERTY_NAME = "uuid";
	private const string NAME_PROPERTY_NAME = "name";
	private const string TEAM_PROPERTY_NAME = "color";

	/// <inheritdoc />
	public override void WriteJson(JsonWriter writer, Player? value, JsonSerializer serializer)
	{
		if (value == null)
			throw new ArgumentNullException(nameof(value));

		writer.WriteStartObject();

		writer.WritePropertyName(UUID_PROPERTY_NAME);
		writer.WriteValue(value.UUID);

		writer.WritePropertyName(NAME_PROPERTY_NAME);
		writer.WriteValue(value.Name);

		writer.WritePropertyName(TEAM_PROPERTY_NAME);
		serializer.Serialize(writer, value.Team);

		writer.WriteEndObject();
	}

	/// <inheritdoc />
	public override Player ReadJson(
		JsonReader reader,
		Type objectType,
		Player? existingValue,
		bool hasExistingValue,
		JsonSerializer serializer
	)
	{
		var obj = (JObject?)serializer.Deserialize(reader);

		if (obj == null)
			throw new JsonException("Expected an object.");

		var uuid = obj.GetRequired<string>(UUID_PROPERTY_NAME, serializer);
		var name = obj.GetRequired<string>(NAME_PROPERTY_NAME, serializer);
		var team = obj.GetRequired<Team>(TEAM_PROPERTY_NAME, serializer);

		return new Player
		{
			UUID = uuid,
			Name = name,
			Team = team,
		};
	}
}
