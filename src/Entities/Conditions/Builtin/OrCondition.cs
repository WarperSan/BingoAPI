using Newtonsoft.Json.Linq;

namespace BingoAPI.Entities.Conditions.Builtin;

/// <summary>
/// Condition that is valid if any of the conditions is valid
/// </summary>
[Condition("OR")]
internal sealed class OrCondition : BaseCondition
{
	private readonly BaseCondition[] _conditions;

	public OrCondition(JObject json) : base(json)
	{
		_conditions = ParseConditions(json);
	}

	/// <inheritdoc/>
	public override bool Check() => _conditions.Any(condition => condition.Check());
}
