using BingoAPI.Goals;
using BingoAPI.Models;
using BingoAPI.Tests.TestObjects;

namespace BingoAPI.Tests.UnitTests.Models;

public class CardTests
{
	[Fact]
	public void Create_WhenNoSquare_ThrowException()
	{
		var pool = new GoalPool();
		var logger = new TestLogger();

		Assert.Throws<ArgumentException>(() =>
		{
			// ReSharper disable once UnusedVariable
			var card = Card.Create([], pool, logger);
		});
	}

	[Theory]
	[InlineData(2)]
	[InlineData(3)]
	[InlineData(5)]
	[InlineData(10)]
	[InlineData(15)]
	[InlineData(101)]
	public void Create_WhenNonSquareAmount_ThrowException(int amount)
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
			var card = Card.Create(squares, pool, logger);
		});
	}

	[Theory]
	[InlineData(-1)]
	[InlineData(10)]
	[InlineData(int.MaxValue)]
	public void Create_WhenIndexOutOfBounds_ThrowException(int index)
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
			var card = Card.Create(squares, pool, logger);
		});
	}
}
