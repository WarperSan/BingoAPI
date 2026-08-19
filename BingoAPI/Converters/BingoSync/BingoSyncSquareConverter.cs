using BingoAPI.Extensions;
using BingoAPI.Models;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace BingoAPI.Converters.BingoSync;

/// <summary>
/// Converts a <see cref="Square"/> from and to a <see cref="string"/>
/// </summary>
public class BingoSyncSquareConverter : JsonConverter<Square>
{
	private const string TEXT_PROPERTY_NAME = "name";
	private const string SLOT_PROPERTY_NAME = "slot";
	private const string TEAMS_PROPERTY_NAME = "colors";

	private const string SLOT_PREFIX = "slot";

	/// <inheritdoc />
	public override void WriteJson(JsonWriter writer, Square? value, JsonSerializer serializer)
	{
		if (value == null)
			throw new ArgumentNullException(nameof(value));

		writer.WriteStartObject();

		writer.WritePropertyName(TEXT_PROPERTY_NAME);
		writer.WriteValue(value.Text);

		writer.WritePropertyName(SLOT_PROPERTY_NAME);
		writer.WriteValue(GetSlotFromIndex(value.Index));

		writer.WritePropertyName(TEAMS_PROPERTY_NAME);
		serializer.Serialize(writer, value.Teams);

		writer.WriteEndObject();
	}

	/// <inheritdoc />
	public override Square ReadJson(
		JsonReader reader,
		Type objectType,
		Square? existingValue,
		bool hasExistingValue,
		JsonSerializer serializer
	)
	{
		var obj = (JObject?)serializer.Deserialize(reader);

		if (obj == null)
			throw new JsonException("Expected an object.");

		var text = obj.GetRequired<string>(TEXT_PROPERTY_NAME, serializer);
		var slot = obj.GetRequired<string>(SLOT_PROPERTY_NAME, serializer);
		var teams = obj.GetRequired<Team>(TEAMS_PROPERTY_NAME, serializer);

		var index = GetIndexFromSlot(slot);

		return new Square
		{
			Text = text,
			Index = index,
			Teams = teams,
		};
	}

	/// <summary>
	/// Gets the index from the given slot
	/// </summary>
	private static int GetIndexFromSlot(string slot)
	{
		if (!slot.StartsWith(SLOT_PREFIX))
			throw new JsonException($"Expected value starting with '{SLOT_PREFIX}'.");

		slot = slot[SLOT_PREFIX.Length..];

		if (!int.TryParse(slot, out var index))
			throw new JsonException($"Could not parse index from '{slot}'.");

		index--;

		if (index < 0)
			throw new JsonException("Index must be greater than 0.");

		return index;
	}

	/// <summary>
	/// Gets the slot from the given index
	/// </summary>
	private static string GetSlotFromIndex(int index)
	{
		if (index < 0)
			throw new ArgumentException("Index cannot be negative.", nameof(index));

		return $"{SLOT_PREFIX}{index + 1}";
	}
}
