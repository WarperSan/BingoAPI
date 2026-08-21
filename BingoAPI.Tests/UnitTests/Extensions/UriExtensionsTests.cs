using BingoAPI.Extensions;

namespace BingoAPI.Tests.UnitTests.Extensions;

public class UriExtensionsTests
{
	[Theory]
	[InlineData("https://bingosync.com/room/6MuWtbUFQE-P70lS6-5BhQ", "6MuWtbUFQE-P70lS6-5BhQ")]
	[InlineData(
		"https://bingosync.com/room/6MuWtbUFQE-P70lS6-5BhQ/?a=10&cs=10",
		"6MuWtbUFQE-P70lS6-5BhQ"
	)]
	[InlineData(
		"https://caravan.kobold60.com/room/DSZViotLRUiBvuQC8WaAZg",
		"DSZViotLRUiBvuQC8WaAZg"
	)]
	[InlineData(
		"https://test.com/public/api/room/DSZViotLRUiBvuQC8WaAZg",
		"DSZViotLRUiBvuQC8WaAZg"
	)]
	public void TryGetRoomCode_WhenFound_ReturnValue(string url, string code)
	{
		var uri = new Uri(url);

		if (!uri.TryGetRoomCode(out var actualCode))
		{
			Assert.Fail();
			return;
		}

		Assert.NotNull(actualCode);
		Assert.Equal(code, actualCode);
	}

	[Theory]
	[InlineData("https://bingosync.com/rooms/6MuWtbUFQE-P70lS6-5BhQ")]
	[InlineData("https://bingosync.com/room?code=6MuWtbUFQE-P70lS6-5BhQ&a=10&cs=10")]
	[InlineData("https://caravan.kobold60.com/rooms/DSZViotLRUiBvuQC8WaAZg")]
	[InlineData("https://test.com/public/api/room/code/DSZViotLRUiBvuQC8WaAZg")]
	public void TryGetRoomCode_WhenNotFound_ReturnNull(string url)
	{
		var uri = new Uri(url);

		if (uri.TryGetRoomCode(out var code))
		{
			Assert.Fail($"Found '{code}' inside '{url}'.");
			return;
		}

		Assert.Null(code);
	}
}
