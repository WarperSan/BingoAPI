namespace BingoAPI.Conditions.BuiltIn;

/// <summary>
/// Condition that is valid when at least the given amount of the conditions are valid
/// </summary>
internal sealed class SomeCondition : ICondition
{
	private readonly ICondition[] _conditions;
	private readonly uint _amount;

	[Condition("SOME")]
	public SomeCondition(ConditionData data)
	{
		_conditions = data.GetRequiredParameter<ICondition[]>("conditions");
		_amount = data.GetOptionalParameter<uint>("amount", 1);
	}

	/// <inheritdoc/>
	public bool IsMet()
	{
		// Skip if always false
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
}
