using System.ComponentModel;
using BingoAPI.Conditions.Attributes;
using BingoAPI.Conditions.Interfaces;
using Newtonsoft.Json;

namespace BingoAPI.Conditions.BuiltIn;

/// <summary>
/// Condition that is valid when the condition is invalid
/// </summary>
[Condition("NOT")]
public sealed class NotCondition : ICondition
{
	/// <summary>
	/// <see cref="ICondition"/> that will be negated
	/// </summary>
	[JsonProperty("condition")]
	[JsonRequired]
	[Description("Condition to negate")]
	public required ICondition Condition { get; init; }

	/// <inheritdoc/>
	public bool IsMet() => !Condition.IsMet();
}
