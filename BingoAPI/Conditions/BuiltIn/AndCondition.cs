using System.ComponentModel;
using BingoAPI.Conditions.Attributes;
using BingoAPI.Conditions.Interfaces;
using Newtonsoft.Json;

namespace BingoAPI.Conditions.BuiltIn;

/// <summary>
/// Condition that is valid when all the conditions are valid
/// </summary>
[Condition("AND")]
public sealed class AndCondition : ICondition
{
	/// <summary>
	/// Every <see cref="ICondition"/> that must be met in order for <see cref="IsMet"/> to return <c>true</c>
	/// </summary>
	[JsonProperty("conditions")]
	[JsonRequired]
	[Description("Conditions that must all be met")]
	public required IReadOnlyCollection<ICondition> Conditions { get; init; }

	/// <inheritdoc/>
	public bool IsMet() => Conditions.All(condition => condition.IsMet());
}
