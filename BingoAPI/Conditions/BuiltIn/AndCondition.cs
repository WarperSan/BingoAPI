using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using Newtonsoft.Json;

namespace BingoAPI.Conditions.BuiltIn;

/// <summary>
/// Condition that is valid when all the conditions are valid
/// </summary>
[Condition("AND")]
[SuppressMessage("ReSharper", "CS1591")]
public sealed class AndCondition : ICondition
{
	[JsonProperty("conditions")]
	[JsonRequired]
	[Description("Conditions that must all be met")]
	public required ICondition[] Conditions { get; init; }

	/// <inheritdoc/>
	public bool IsMet() => Conditions.All(condition => condition.IsMet());
}
