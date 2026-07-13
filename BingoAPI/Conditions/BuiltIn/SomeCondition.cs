using System.ComponentModel;
using Newtonsoft.Json;

namespace BingoAPI.Conditions.BuiltIn;

/// <summary>
/// Condition that is valid when at least the given amount of the conditions are valid
/// </summary>
[Condition("SOME")]
internal sealed class SomeCondition : ICondition
{
	[JsonProperty("conditions")]
	[JsonRequired]
	[Description("Conditions that could be met")]
	public required ICondition[] Conditions { get; init; }

	[JsonProperty("amount")]
	[DefaultValue(1)]
	[Description("Minimum number of conditions that must be met")]
	public uint Amount { get; init; }

	/// <inheritdoc/>
	public bool IsMet()
	{
		// Skip if always false
		if (Conditions.Length < Amount)
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
