namespace BingoAPI.Conditions.BuiltIn;

/// <summary>
/// Condition that is valid when the condition is invalid
/// </summary>
internal sealed class NotCondition : ICondition
{
	private readonly ICondition _condition;

	private NotCondition(ICondition condition)
	{
		_condition = condition;
	}

	/// <inheritdoc/>
	public bool IsMet() => !_condition.IsMet();

	public static ICondition Create(ConditionData data)
	{
		var child = data.GetChild();

		return new NotCondition(child);
	}
}
