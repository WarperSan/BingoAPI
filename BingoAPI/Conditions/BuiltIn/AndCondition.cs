namespace BingoAPI.Conditions.BuiltIn;

/// <summary>
/// Condition that is valid when all the conditions are valid
/// </summary>
internal sealed class AndCondition : ICondition
{
	private readonly ICondition[] _conditions;

	private AndCondition(ICondition[] conditions)
	{
		_conditions = conditions;
	}

	/// <inheritdoc/>
	public bool IsMet() => _conditions.All(condition => condition.IsMet());

	public static ICondition Create(ConditionData data)
	{
		var conditions = data.GetRequiredParameter<ICondition[]>("conditions");

		return new AndCondition(conditions);
	}
}
