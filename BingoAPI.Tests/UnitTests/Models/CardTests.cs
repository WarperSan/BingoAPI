using BingoAPI.Goals;
using BingoAPI.Models;
using BingoAPI.Tests.TestObjects;

namespace BingoAPI.Tests.UnitTests.Models;

public class CardTests
{
	[Fact]
	public void ctor_WhenNoSquare_ThrowException()
	{
		var pool = new GoalPool();
		var logger = new TestLogger();

		Assert.Throws<ArgumentException>(() =>
		{
			// ReSharper disable once UnusedVariable
			var card = new Card([], pool, logger);
		});
	}

	[Theory]
	[InlineData(2)]
	[InlineData(3)]
	[InlineData(5)]
	[InlineData(10)]
	[InlineData(15)]
	[InlineData(101)]
	public void ctor_WhenNonSquareAmount_ThrowException(int amount)
	{
		var square = new Square
		{
			Text = "",
			Index = 0,
			Teams = Team.None,
		};

		var squares = new Square[amount];

		for (var i = 0; i < squares.Length; i++)
			squares[i] = square;

		var pool = new GoalPool();
		var logger = new TestLogger();

		Assert.Throws<ArgumentException>(() =>
		{
			// ReSharper disable once UnusedVariable
			var card = new Card(squares, pool, logger);
		});
	}

	[Theory]
	[InlineData(-1)]
	[InlineData(10)]
	[InlineData(int.MaxValue)]
	public void ctor_WhenIndexOutOfBounds_ThrowException(int index)
	{
		var squares = new Square[]
		{
			new()
			{
				Text = "",
				Index = index,
				Teams = Team.None,
			},
		};

		var pool = new GoalPool();
		var logger = new TestLogger();

		Assert.Throws<ArgumentOutOfRangeException>(() =>
		{
			// ReSharper disable once UnusedVariable
			var card = new Card(squares, pool, logger);
		});
	}
}
