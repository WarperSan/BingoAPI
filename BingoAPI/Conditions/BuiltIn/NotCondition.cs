namespace BingoAPI.Conditions.BuiltIn;

/// <summary>
/// Condition that is valid when the condition is invalid
/// </summary>
internal sealed class NotCondition : ICondition
{
	private readonly ICondition _condition;

	public NotCondition(ConditionData data)
	{
		_condition = data.GetRequiredParameter<ICondition>("condition");
	}

	/// <inheritdoc/>
	public bool IsMet() => !_condition.IsMet();
}
