using BingoAPI.Extensions;
using Newtonsoft.Json.Linq;

namespace BingoAPI.Entities.Conditions.Builtin;

/// <summary>
/// Condition that is valid when at least the given amount of the conditions are valid
/// </summary>
[Condition("SOME")]
internal sealed class SomeCondition : BaseCondition
{
	private readonly uint _amount;
	private readonly BaseCondition[] _conditions;

	public SomeCondition(JObject json) : base(json)
	{
		_conditions = ParseConditions(json);

		var parameters = ParseParameters(json);

		_amount = parameters.GetRequiredValue<uint>("amount");
	}

	/// <inheritdoc/>
	public override bool Check()
	{
		if (_conditions.Length < _amount)
			return false;

		var currentAmount = 0;

		foreach (var condition in _conditions)
		{
			if (!condition.Check())
				continue;

			currentAmount++;

			if (currentAmount >= _amount)
				return true;
		}

		return false;
	}
}
