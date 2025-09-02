using Newtonsoft.Json.Linq;

namespace BingoAPI.Entities.Conditions.Composites;

/// <summary>
/// Condition that is valid if any of the conditions is valid
/// </summary>
internal sealed class OrCondition : BaseCompositeCondition
{
    public OrCondition(JObject json) : base(json)
    {
    }

    /// <inheritdoc/>
    public override bool Check()
    {
        foreach (var condition in Conditions)
        {
            if (condition.Check())
                return true;
        }

        return false;
    }
}