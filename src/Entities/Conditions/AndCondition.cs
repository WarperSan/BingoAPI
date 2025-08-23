using Newtonsoft.Json.Linq;

namespace BingoAPI.Entities.Conditions;

/// <summary>
/// Condition that is valid when all the conditions are valid
/// </summary>
internal sealed class AndCondition : BaseCondition
{
    private readonly BaseCondition[] Conditions;

    public AndCondition(JObject json) : base(json)
    {
        Conditions = ParseConditions(json);
    }

    /// <inheritdoc/>
    public override bool Check()
    {
        foreach (var condition in Conditions)
        {
            if (!condition.Check())
                return false;
        }

        return true;
    }
}