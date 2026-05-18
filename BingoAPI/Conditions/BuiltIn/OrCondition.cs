namespace BingoAPI.Conditions.BuiltIn;

/// <summary>
/// Condition that is valid if any of the conditions is valid
/// </summary>
internal sealed class OrCondition : ICondition
{
	private readonly ICondition[] _conditions;

	private OrCondition(ICondition[] conditions)
	{
		_conditions = conditions;
	}

	/// <inheritdoc/>
	public bool IsMet() => _conditions.Any(condition => condition.IsMet());

	public static ICondition Create(ConditionData data)
	{
		var children = data.GetChildren();

		return new OrCondition(children);
	}
}
