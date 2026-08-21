using BingoAPI.Extensions;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace BingoAPI.Tests.UnitTests.Extensions;

public class JsonExtensionsTests
{
	[Fact]
	public void GetRequired_WhenMissing_ThrowException()
	{
		const string PROPERTY_NAME = "taxes";

		var payload = new Dictionary<string, object>();

		var jObject = JObject.FromObject(payload);

		Assert.Throws<JsonException>(() =>
		{
			// ReSharper disable once UnusedVariable
			var property = jObject.GetRequired(PROPERTY_NAME);
		});
	}

	[Theory]
	[InlineData(null)]
	[InlineData(10)]
	[InlineData("10")]
	[InlineData(2.2f)]
	public void GetRequired_WhenFound_ReturnsToken(object? value)
	{
		const string PROPERTY_NAME = "taxes";

		var payload = new Dictionary<string, object?> { { PROPERTY_NAME, value } };

		var jObject = JObject.FromObject(payload);
		var property = jObject.GetRequired(PROPERTY_NAME);

		var actualValue = property.Value<JValue>()?.Value;

		Assert.Equal(value, actualValue);
	}

	[Fact]
	public void GetRequiredGeneric_WhenNull_ThrowException()
	{
		const string PROPERTY_NAME = "taxes";

		var payload = new Dictionary<string, object?> { { PROPERTY_NAME, null } };

		var jObject = JObject.FromObject(payload);

		Assert.Throws<JsonException>(() =>
		{
			// ReSharper disable once UnusedVariable
			var property = jObject.GetRequired<string>(
				PROPERTY_NAME,
				JsonSerializer.CreateDefault()
			);
		});
	}
}
