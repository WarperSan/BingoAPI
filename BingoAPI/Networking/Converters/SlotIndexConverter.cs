using BingoAPI.Networking.DTOs;
using Newtonsoft.Json;

namespace BingoAPI.Networking.Converters;

/// <summary>
/// Converts a 0-based <see cref="int"/> index to and from a 1-based <see cref="string"/> slot index
/// </summary>
internal class SlotIndexConverter : JsonConverter<SlotIndex>
{
	private const string PREFIX = "slot";

	/// <inheritdoc />
	public override void WriteJson(JsonWriter writer, SlotIndex? value, JsonSerializer serializer)
	{
		var index = 0;

		if (value != null)
			index = value.Index + 1;

		writer.WriteValue($"{PREFIX}{index}");
	}

	/// <inheritdoc />
	public override SlotIndex ReadJson(
		JsonReader reader,
		Type objectType,
		SlotIndex? existingValue,
		bool hasExistingValue,
		JsonSerializer serializer
	)
	{
		if (reader.Value is not string rawIndex)
			throw new JsonException($"Expected a '{nameof(String)}', but  got '{reader.ValueType}'.");

		if (!rawIndex.StartsWith(PREFIX))
			throw new JsonException($"Expected value starting with '{PREFIX}'.");

		rawIndex = rawIndex[PREFIX.Length..];

		if (!int.TryParse(rawIndex, out var index))
			throw new JsonException($"Could not parse index from '{rawIndex}'.");

		if (index <= 0)
			throw new JsonException("Index must be greater than 0.");

		return new SlotIndex(index - 1);
	}
}
