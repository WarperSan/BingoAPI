using Newtonsoft.Json.Linq;

namespace BingoAPI.Entities.Conditions;

/// <summary>
/// Condition that is valid if any of the conditions is valid
/// </summary>
internal sealed class OrCondition : BaseCondition
{
    private readonly BaseCondition?[] Conditions;

    public OrCondition(JObject obj) : base(obj)
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

            if (condition.Check())
                return true;
        }

        return false;
    }
}