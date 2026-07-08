namespace BingoAPI.Conditions.BuiltIn;

/// <summary>
/// Condition that is valid if any of the conditions is valid
/// </summary>
[Condition("OR")]
internal sealed class OrCondition : ICondition
{
	private readonly ICondition[] _conditions;

	public OrCondition(ConditionData data)
	{
		_conditions = data.GetRequiredParameter<ICondition[]>("conditions");
	}

	/// <inheritdoc/>
	public bool IsMet() => _conditions.Any(condition => condition.IsMet());
}
