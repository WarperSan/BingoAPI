using Newtonsoft.Json.Linq;

namespace BingoAPI.Entities.Conditions.Builtin;

/// <summary>
/// Condition that is valid when all the conditions are valid
/// </summary>
[Condition("AND")]
internal sealed class AndCondition : BaseCondition
{
    private readonly BaseCondition[] _conditions;
    
    public AndCondition(JObject json) : base(json)
    {
        _conditions = ParseConditions(json);
    }

    /// <inheritdoc/>
    public override bool Check() => _conditions.All(condition => condition.Check());
}