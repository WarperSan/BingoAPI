using System;
using Newtonsoft.Json.Linq;

namespace BingoAPI.Entities.Conditions.Builtin;

/// <summary>
/// Condition that is valid when the condition is invalid
/// </summary>
[Condition("NOT")]
internal sealed class NotCondition : BaseCondition
{
    private readonly BaseCondition _condition;

    public NotCondition(JObject json) : base(json)
    {
        var conditions = ParseConditions(json);

        if (conditions.Length < 1)
            throw new ArgumentException("Expected at least one condition.");

        _condition = conditions[0];
    }

    /// <inheritdoc/>
    public override bool Check() => !_condition.Check();
}