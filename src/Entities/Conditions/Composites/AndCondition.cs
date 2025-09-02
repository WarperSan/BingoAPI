using Newtonsoft.Json.Linq;

namespace BingoAPI.Entities.Conditions.Composites;

/// <summary>
/// Condition that is valid when all the conditions are valid
/// </summary>
internal sealed class AndCondition : BaseCompositeCondition
{
    public AndCondition(JObject json) : base(json)
    {
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