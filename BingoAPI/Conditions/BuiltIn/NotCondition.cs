using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using Newtonsoft.Json;

namespace BingoAPI.Conditions.BuiltIn;

/// <summary>
/// Condition that is valid when the condition is invalid
/// </summary>
[Condition("NOT")]
[SuppressMessage("ReSharper", "CS1591")]
public sealed class NotCondition : ICondition
{
	[JsonProperty("condition")]
	[JsonRequired]
	[Description("Condition to inverse")]
	public required ICondition Condition { get; init; }

	/// <inheritdoc/>
	public bool IsMet() => !Condition.IsMet();
}
