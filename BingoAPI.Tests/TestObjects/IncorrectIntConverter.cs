using Newtonsoft.Json;

namespace BingoAPI.Tests.TestObjects;

/// <summary>
/// Converts a <see cref="int"/> to another <see cref="int"/> incorrectly
/// </summary>
/// <remarks>
/// This is used when needing to process a custom <see cref="JsonConverter"/>
/// </remarks>
internal sealed class IncorrectIntConverter : JsonConverter<int>
{
	/// <inheritdoc />
	public override void WriteJson(JsonWriter writer, int value, JsonSerializer serializer)
	{
		writer.WriteValue(value + 1);
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
		var value = reader.ReadAsInt32();

		if (!value.HasValue)
			throw new JsonException("Cannot convert null value to int.");

		return value.Value - 1;
	}
}
