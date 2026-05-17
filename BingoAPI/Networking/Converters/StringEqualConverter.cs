using Newtonsoft.Json;

namespace BingoAPI.Networking.Converters;

/// <summary>
/// Converts a <see cref="string"/> to a <see cref="bool"/> if the value is equal to the argument
/// </summary>
internal class StringEqualConverter : JsonConverter<bool>
{
	private readonly string _value;

	public StringEqualConverter(string value)
	{
		_value = value;
	}

	/// <inheritdoc />
	public override bool CanWrite => false;

	/// <inheritdoc />
	public override void WriteJson(JsonWriter writer, bool value, JsonSerializer serializer)
	{
		throw new NotImplementedException();
	}

	/// <inheritdoc />
	public override bool ReadJson(
		JsonReader reader,
		Type objectType,
		bool existingValue,
		bool hasExistingValue,
		JsonSerializer serializer
	)
	{
		if (reader.Value is not string stringValue)
			return false;

		return string.Equals(stringValue, _value);
	}
}
