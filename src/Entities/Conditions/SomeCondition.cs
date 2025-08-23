using BingoAPI.Extensions;
using Newtonsoft.Json.Linq;

namespace BingoAPI.Entities.Conditions;

/// <summary>
/// Condition that is valid when at least the given amount of the conditions are valid
/// </summary>
internal sealed class SomeCondition : BaseCondition
{
    private readonly BaseCondition?[] Conditions;

    private readonly uint amount;

    public SomeCondition(JObject obj) : base(obj)
    {
        var parameters = ParseParameters(obj);

        amount = parameters.GetValueOrDefault("amount", 1u);
        
        Conditions = ParseConditions(obj);
    }

    /// <inheritdoc/>
    public override bool Check()
    {
        var currentAmount = 0;

        foreach (var condition in Conditions)
        {
            if (condition == null)
                continue;

            if (!condition.Check())
                continue;

            currentAmount++;

            if (currentAmount >= amount)
                return true;
        }

        return false;
    }
}