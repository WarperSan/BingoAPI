using Newtonsoft.Json.Linq;

namespace BingoAPI.Entities.Conditions;

/// <summary>
/// Condition that is valid when all the conditions are valid
/// </summary>
internal sealed class AndCondition : BaseCondition
{
    private readonly BaseCondition?[] Conditions;

    public AndCondition(JObject obj) : base(obj)
    {
        Conditions = ParseConditions(obj);
    }

    /// <inheritdoc/>
    public override bool Check()
    {
        foreach (var condition in Conditions)
        {
            if (condition == null)
                continue;

            if (!condition.Check())
                return false;
        }

        return true;
    }
}