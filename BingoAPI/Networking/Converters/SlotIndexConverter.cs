using Newtonsoft.Json;

namespace BingoAPI.Networking.Converters;

/// <summary>
/// Converts a 0-based <see cref="int"/> index to and from a 1-based <see cref="string"/> slot index
/// </summary>
internal class SlotIndexConverter : JsonConverter<int>
{
	/// <inheritdoc />
	public override void WriteJson(JsonWriter writer, int value, JsonSerializer serializer)
	{
		writer.WriteValue($"slot{value + 1}");
	}

	/// <inheritdoc />
	public override int ReadJson(
		JsonReader reader,
		Type objectType,
		int existingValue,
		bool hasExistingValue,
		JsonSerializer serializer
	)
	{
		if (reader.Value is not string rawIndex)
			throw new JsonException($"Expected a '{nameof(String)}', but  got '{reader.ValueType}'.");

		rawIndex = rawIndex.Replace("slot", "");

		// ReSharper disable once ConvertIfStatementToReturnStatement
		if (!int.TryParse(rawIndex, out var index))
			throw new JsonException($"Could not parse index from '{rawIndex}'.");

		return index - 1;
	}
}
