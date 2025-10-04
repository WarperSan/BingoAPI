using BingoAPI.Extensions;
using Newtonsoft.Json.Linq;

namespace BingoAPI.Entities.Conditions.Composites;

/// <summary>
/// Condition that is valid when at least the given amount of the conditions are valid
/// </summary>
internal sealed class SomeCondition : BaseCompositeCondition
{
    private readonly uint amount;

    public SomeCondition(JObject json) : base(json)
    {
        var parameters = ParseParameters(json);

        amount = parameters.GetRequiredValue<uint>("amount");
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