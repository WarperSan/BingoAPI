using BingoAPI.Models;
using Newtonsoft.Json;

namespace BingoAPI.Tests.UnitTests.Models;

public class TeamTests
{
	private const string BLANK_STRING = "blank";
	private const string PINK_STRING = "pink";
	private const string NAVY_STRING = "navy";
	private const string PURPLE_STRING = "purple";

	[Theory]
	[InlineData(Team.None, BLANK_STRING)]
	[InlineData(Team.Pink, PINK_STRING)]
	[InlineData(Team.Navy, NAVY_STRING)]
	[InlineData(Team.Purple, PURPLE_STRING)]
	public void SerializeSingle_WhenCalled_ReturnExcepted(Team team, string expected)
	{
		var result = JsonConvert.SerializeObject(team);

		if (!result.StartsWith('"') || !result.EndsWith('"'))
			Assert.Fail($"Failed to serialize '{nameof(Team)}' into a proper JSON string.");

		result = result[1..^1];

		Assert.Equal(expected, result);
	}

	[Theory]
	[InlineData(Team.Navy | Team.Pink, NAVY_STRING, PINK_STRING)]
	[InlineData(Team.Pink | Team.Navy, PINK_STRING, NAVY_STRING)]
	[InlineData(Team.None | Team.Pink, PINK_STRING)]
	[InlineData(Team.Purple | Team.Pink, PINK_STRING, PURPLE_STRING)]
	[InlineData(Team.Purple | Team.Pink | Team.Navy, NAVY_STRING, PINK_STRING, PURPLE_STRING)]
	public void SerializeMultiple_WhenCalled_ReturnExpected(Team team, params string[] expected)
	{
		var result = JsonConvert.SerializeObject(team);

		if (!result.StartsWith('"') || !result.EndsWith('"'))
			Assert.Fail($"Failed to serialize '{nameof(Team)}' into a proper JSON string.");

		result = result[1..^1];

		var parts = result.Split(' ');

		Assert.Equal(expected.Length, parts.Length);

		foreach (var expectedPart in expected)
			Assert.Contains(expectedPart, parts);
	}
}
