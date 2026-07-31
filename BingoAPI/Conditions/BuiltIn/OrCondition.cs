using System.ComponentModel;
using BingoAPI.Conditions.Attributes;
using BingoAPI.Conditions.Interfaces;
using Newtonsoft.Json;

namespace BingoAPI.Conditions.BuiltIn;

/// <summary>
/// Condition that is valid if any of the conditions is valid
/// </summary>
[Condition("OR")]
public sealed class OrCondition : ICondition
{
	/// <summary>
	/// Every <see cref="ICondition"/> that could be met in order for <see cref="IsMet"/> to return <c>true</c>
	/// </summary>
	[JsonProperty("conditions")]
	[JsonRequired]
	[Description("Conditions where at least one must be met")]
	public required IReadOnlyCollection<ICondition> Conditions { get; init; }

	/// <inheritdoc/>
	public bool IsMet() => Conditions.Any(condition => condition.IsMet());
}
