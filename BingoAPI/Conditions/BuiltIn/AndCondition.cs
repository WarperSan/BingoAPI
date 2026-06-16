namespace BingoAPI.Conditions.BuiltIn;

/// <summary>
/// Condition that is valid when all the conditions are valid
/// </summary>
internal sealed class AndCondition : ICondition
{
	private readonly ICondition[] _conditions;

	[Condition("AND")]
	public AndCondition(ConditionData data)
	{
		_conditions = data.GetRequiredParameter<ICondition[]>("conditions");
	}

	/// <inheritdoc/>
	public bool IsMet() => _conditions.All(condition => condition.IsMet());
}
