using Newtonsoft.Json;

namespace BingoAPI.Conditions.BuiltIn;

/// <summary>
/// Condition that is valid if any of the conditions is valid
/// </summary>
[Condition("OR")]
internal sealed class OrCondition : ICondition
{
	[JsonProperty("conditions")]
	[JsonRequired]
	public required ICondition[] Conditions { get; init; }

	/// <inheritdoc/>
	public bool IsMet() => Conditions.Any(condition => condition.IsMet());
}
