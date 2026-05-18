namespace BingoAPI.Conditions.BuiltIn;

/// <summary>
/// Condition that is valid when at least the given amount of the conditions are valid
/// </summary>
internal sealed class SomeCondition : ICondition
{
	private readonly uint _amount;
	private readonly ICondition[] _conditions;

	private SomeCondition(uint amount, params ICondition[] conditions)
	{
		_amount = amount;
		_conditions = conditions;
	}

	/// <inheritdoc/>
	public bool IsMet()
	{
		if (_conditions.Length < _amount)
			return false;

		var currentAmount = 0;

		foreach (var condition in _conditions)
		{
			if (!condition.IsMet())
				continue;

			currentAmount++;

			if (currentAmount >= _amount)
				return true;
		}

		return false;
	}

	public static ICondition Create(ConditionData data)
	{
		var children = data.GetChildren();
		var amount = data.GetOptionalParam<uint>("amount", 1);

		return new SomeCondition(amount, children);
	}
}
