using BingoAPI.Conditions;

namespace BingoAPI.Conditions;

/// <summary>
/// Condition that is valid when all the conditions are valid
/// </summary>
internal sealed class AndCondition : ICondition
{
	private readonly ICondition[] _conditions;

	public AndCondition(ICondition[] conditions)
	{
		_conditions = conditions;
	}

	/// <inheritdoc/>
	public bool Check() => _conditions.All(condition => condition.Check());
}
