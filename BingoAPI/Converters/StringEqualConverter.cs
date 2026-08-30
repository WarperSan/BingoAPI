using Newtonsoft.Json;

namespace BingoAPI.Converters;

/// <summary>
/// Converts a <see cref="string"/> to a <see cref="bool"/> if the value is equal to the argument
/// </summary>
internal sealed class StringEqualConverter : JsonConverter<bool>
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
		throw new InvalidOperationException(
			$"Class '{nameof(StringEqualConverter)}' cannot write a '{typeof(bool)}' as '{_value.GetType()}'."
		);
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
		if (reader.Value is not string rawValue)
			throw new JsonException(
				$"Expected a '{typeof(string)}', but got '{reader.ValueType}'."
			);

		return rawValue.Equals(_value);
	}
}
