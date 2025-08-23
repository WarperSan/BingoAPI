using BingoAPI.Extensions;
using Newtonsoft.Json.Linq;

namespace BingoAPI.Entities.Conditions;

/// <summary>
/// Condition that is valid when at least the given amount of the conditions are valid
/// </summary>
internal sealed class SomeCondition : BaseCondition
{
    private readonly BaseCondition[] Conditions;

    private readonly uint amount;

    public SomeCondition(JObject json) : base(json)
    {
        var parameters = ParseParameters(json);

        amount = parameters.GetValueOrDefault("amount", 1u);
        
        Conditions = ParseConditions(json);
    }

    /// <inheritdoc/>
    public override bool Check()
    {
        if (Conditions.Length < amount)
            return false;

        var currentAmount = 0;

        foreach (var condition in Conditions)
        {
            if (!condition.Check())
                continue;

            currentAmount++;

            if (currentAmount >= amount)
                return true;
        }

        return false;
    }
}