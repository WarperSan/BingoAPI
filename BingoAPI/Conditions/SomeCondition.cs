using BingoAPI.Conditions;

namespace BingoAPI.Conditions;

/// <summary>
/// Condition that is valid when at least the given amount of the conditions are valid
/// </summary>
internal sealed class SomeCondition : ICondition
{
	private readonly uint _amount;
	private readonly ICondition[] _conditions;

	public SomeCondition(uint amount, params ICondition[] conditions)
	{
		_amount = amount;
		_conditions = conditions;
	}

	/// <inheritdoc/>
	public bool Check()
	{
		if (_conditions.Length < _amount)
			return false;

		var currentAmount = 0;

		foreach (var condition in _conditions)
		{
			if (!condition.Check())
				continue;

			currentAmount++;

			if (currentAmount >= _amount)
				return true;
		}

		return false;
	}
}
