using BingoAPI.Converters;
using Newtonsoft.Json;

namespace BingoAPI.Tests.UnitTests.Converters;

public class StringEqualConverterTests
{
	[Theory]
	[InlineData("abc", "abc")]
	public void Read_WhenEqual_ReturnTrue(string equalValue, string rawValue)
	{
		var converter = new StringEqualConverter(equalValue);

		if (!converter.CanRead)
		{
			Assert.Fail();
			return;
		}

		var serializer = new JsonSerializer { Converters = { converter } };

		var json = JsonConvert.SerializeObject(rawValue);
		var reader = new JsonTextReader(new StringReader(json));

		reader.Read();

		var actualValue = converter.ReadJson(reader, typeof(object), null, serializer) as bool?;

		Assert.NotNull(actualValue);
		Assert.True(actualValue);
	}

	[Theory]
	[InlineData("abc", "cba")]
	[InlineData("DdD", "dDd")]
	[InlineData("123321", "123")]
	public void Read_WhenNotEqual_ReturnFalse(string equalValue, string rawValue)
	{
		Assert.NotEqual(equalValue, rawValue);

		var converter = new StringEqualConverter(equalValue);

		if (!converter.CanRead)
		{
			Assert.Fail();
			return;
		}

		var serializer = new JsonSerializer { Converters = { converter } };

		var json = JsonConvert.SerializeObject(rawValue);
		var reader = new JsonTextReader(new StringReader(json));

		reader.Read();

		var actualValue = converter.ReadJson(reader, typeof(object), null, serializer) as bool?;

		Assert.NotNull(actualValue);
		Assert.False(actualValue);
	}
}
