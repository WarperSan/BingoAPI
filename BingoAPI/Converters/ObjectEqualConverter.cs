using Newtonsoft.Json;

namespace BingoAPI.Converters;

/// <summary>
/// Converts a <see cref="object"/> to a <see cref="bool"/> if the value is equal to the argument
/// </summary>
internal sealed class ObjectEqualConverter : JsonConverter<bool>
{
	private readonly object _value;

	public ObjectEqualConverter(object value)
	{
		_value = value;
	}

	/// <inheritdoc />
	public override bool CanWrite => false;

	/// <inheritdoc />
	public override void WriteJson(JsonWriter writer, bool value, JsonSerializer serializer)
	{
		throw new InvalidOperationException(
			$"Class '{nameof(ObjectEqualConverter)}' cannot write a '{typeof(bool)}' as '{_value.GetType()}'."
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
		if (reader.Value is null)
			return false;

		return reader.Value.Equals(_value);
	}
}
