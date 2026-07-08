using Newtonsoft.Json;

namespace BingoAPI.Conditions.BuiltIn;

/// <summary>
/// Condition that is valid when all the conditions are valid
/// </summary>
[Condition("AND")]
internal sealed class AndCondition : ICondition
{
	[JsonProperty("conditions")]
	[JsonRequired]
	public required ICondition[] Conditions { get; init; }

	/// <inheritdoc/>
	public bool IsMet() => Conditions.All(condition => condition.IsMet());
}
