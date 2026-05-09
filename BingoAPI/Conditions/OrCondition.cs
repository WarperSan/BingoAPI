namespace BingoAPI.Conditions;

/// <summary>
/// Condition that is valid if any of the conditions is valid
/// </summary>
internal sealed class OrCondition : ICondition
{
	private readonly ICondition[] _conditions;

	public OrCondition(ICondition[] conditions)
	{
		_conditions = conditions;
	}

	/// <inheritdoc/>
	public bool Check() => _conditions.Any(condition => condition.Check());
}
