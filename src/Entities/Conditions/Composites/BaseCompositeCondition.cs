using Newtonsoft.Json.Linq;

namespace BingoAPI.Entities.Conditions.Composites;

/// <summary>
/// Class that represents any condition with any child attached to it
/// </summary>
public abstract class BaseCompositeCondition : BaseCondition
{
    /// <summary>
    /// Conditions attached to this one
    /// </summary>
    protected readonly BaseCondition[] Conditions;

    /// <inheritdoc/>
    protected BaseCompositeCondition(JObject json) : base(json)
    {
        Conditions = ParseConditions(json);
    }
}