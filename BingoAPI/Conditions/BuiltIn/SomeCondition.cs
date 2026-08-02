using System.ComponentModel;
using BingoAPI.Conditions.Attributes;
using BingoAPI.Conditions.Interfaces;
using Newtonsoft.Json;

namespace BingoAPI.Conditions.BuiltIn;

/// <summary>
/// Condition that is valid when at least the given amount of the conditions are valid
/// </summary>
[Condition("SOME")]
public sealed class SomeCondition : ICondition
{
	/// <summary>
	/// Every <see cref="ICondition"/> that will be checked until the threshold is met
	/// </summary>
	[JsonProperty("conditions")]
	[JsonRequired]
	[Description("Conditions that could be met")]
	public required IReadOnlyCollection<ICondition> Conditions { get; init; }

	/// <summary>
	/// Minimum amount of items in <see cref="Conditions"/> that must be met in order for <see cref="IsMet"/> to return <c>true</c>
	/// </summary>
	[JsonProperty("amount")]
	[DefaultValue(2)]
	[Description("Minimum number of conditions that must be met")]
	public uint Amount { get; init; }

	/// <inheritdoc/>
	public bool IsMet()
	{
		// Skip if always false
		if (Conditions.Count < Amount)
			return false;

		var currentAmount = 0;

		foreach (var condition in Conditions)
		{
			if (!condition.IsMet())
				continue;

			currentAmount++;

			if (currentAmount >= Amount)
				return true;
		}

		return false;
	}
}
