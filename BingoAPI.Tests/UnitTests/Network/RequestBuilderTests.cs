using BingoAPI.Network;
using BingoAPI.Tests.TestObjects;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace BingoAPI.Tests.UnitTests.Network;

// ReSharper disable InconsistentNaming

public class RequestBuilderTests
{
	public static TheoryData<HttpMethod> SetMethodData =>
		[HttpMethod.Get, HttpMethod.Post, HttpMethod.Put];

	[Theory]
	[MemberData(nameof(SetMethodData))]
	public void SetMethod(HttpMethod method)
	{
		var builder = new RequestBuilder();

		switch (method.Method)
		{
			case "GET":
				builder.Get();
				break;
			case "POST":
				builder.Post();
				break;
			case "PUT":
				builder.Put();
				break;
			default:
				throw new InvalidOperationException($"Method '{method.Method}' is not valid.");
		}

		var request = builder.Build();
		Assert.Equal(method, request.Method);
	}

	[Theory]
	[InlineData("https://www.google.com")]
	[InlineData("https://www.google.com/?a=1")]
	[InlineData("https://google.com")]
	public void ToUri_WhenSet_AssignUri(string url)
	{
		var uri = new Uri(url);
		var request = new RequestBuilder().ToUri(uri).Build();

		Assert.NotNull(request.RequestUri);
		Assert.Equal(uri.AbsoluteUri, request.RequestUri.AbsoluteUri);
	}

	[Theory]
	[InlineData("/apple/com")]
	[InlineData("/https/www/google/com")]
	[InlineData("/api/experimental/submission/package/")]
	public void ToEndpoint_WhenSet_AssignEndpoint(string endpoint)
	{
		var uri = new Uri("https://www.google.com");
		var request = new RequestBuilder().ToUri(uri).ToEndpoint(endpoint).Build();

		Assert.NotNull(request.RequestUri);
		Assert.Equal(endpoint, request.RequestUri.AbsolutePath);
	}

	public static TheoryData<object> WithJson_WhenSet_AssignJsonContent_Data =>
		[new { baba = 2 }, new { ababa = (string[])["1", "2"] }];

	[Theory]
	[MemberData(nameof(WithJson_WhenSet_AssignJsonContent_Data))]
	public async Task WithJson_WhenSetWithoutSettings_AssignJsonContentWithDefault(object payload)
	{
		var request = new RequestBuilder().WithJson(payload).Build();

		Assert.NotNull(request.Content);

		var actualPayload = await request.Content.ReadAsStringAsync(
			TestContext.Current.CancellationToken
		);

		var expectedJson = JObject.FromObject(payload);
		var actualJson = JsonConvert.DeserializeObject(actualPayload);

		Assert.NotSame(expectedJson, actualJson);
		Assert.Equal(expectedJson, actualJson);
	}

	[Theory]
	[MemberData(nameof(WithJson_WhenSet_AssignJsonContent_Data))]
	public async Task WithJson_WhenSetWithSettings_AssignJsonContentWithSettings(object payload)
	{
		var serializerSettings = new JsonSerializerSettings
		{
			Converters = [new IncorrectIntConverter()],
		};

		var request = new RequestBuilder().WithJson(payload, serializerSettings).Build();

		Assert.NotNull(request.Content);

		var actualPayload = await request.Content.ReadAsStringAsync(
			TestContext.Current.CancellationToken
		);

		var expectedJson = JObject.FromObject(payload);
		var actualJson = JsonConvert.DeserializeObject(actualPayload, serializerSettings);

		Assert.NotSame(expectedJson, actualJson);
		Assert.Equal(expectedJson, actualJson);
	}

	[Fact]
	public void Copy_WhenCopied_ReturnNewInstance()
	{
		var originalBuilder = new RequestBuilder();
		var copiedBuilder = new RequestBuilder(originalBuilder);

		Assert.NotSame(copiedBuilder, originalBuilder);
	}

	[Fact]
	public void Copy_WhenCopiedAndModified_OriginalStaysUntouched()
	{
		var originalBuilder = new RequestBuilder().Post();
		var copiedBuilder = new RequestBuilder(originalBuilder).Get();

		var originalRequest = originalBuilder.Build();
		var copiedRequest = copiedBuilder.Build();

		Assert.Equal(HttpMethod.Post, originalRequest.Method);
		Assert.Equal(HttpMethod.Get, copiedRequest.Method);
	}

	[Fact]
	public void Copy_WhenCopiedWithMethod_ReturnNewInstanceWithMethod()
	{
		var originalBuilder = new RequestBuilder().Post();
		var copiedBuilder = new RequestBuilder(originalBuilder);

		var originalRequest = originalBuilder.Build();
		var copiedRequest = copiedBuilder.Build();

		Assert.Equal(originalRequest.Method, copiedRequest.Method);
	}

	[Fact]
	public async Task Copy_WhenCopiedWithContent_ReturnNewInstanceWithContent()
	{
		// Arrange
		var payload = new
		{
			test = 1,
			test2 = 2,
			test3 = 3,
			test4 = 4,
		};

		var originalBuilder = new RequestBuilder().WithJson(payload);
		var copiedBuilder = new RequestBuilder(originalBuilder);

		// Act
		var originalRequest = originalBuilder.Build();
		var copiedRequest = copiedBuilder.Build();

		// Assert
		Assert.NotNull(originalRequest.Content);

		var originalPayload = await originalRequest.Content.ReadAsStringAsync(
			TestContext.Current.CancellationToken
		);

		Assert.NotNull(copiedRequest.Content);

		var copiedPayload = await copiedRequest.Content.ReadAsStringAsync(
			TestContext.Current.CancellationToken
		);

		var expectedJson = JObject.FromObject(payload);
		var actualOriginalPayload = JsonConvert.DeserializeObject(originalPayload);
		var actualCopiedPayload = JsonConvert.DeserializeObject(copiedPayload);

		Assert.Equal(expectedJson, actualOriginalPayload);
		Assert.Equal(expectedJson, actualCopiedPayload);
		Assert.NotSame(actualOriginalPayload, actualCopiedPayload);
	}
}
