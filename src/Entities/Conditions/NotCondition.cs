using System;
using Newtonsoft.Json.Linq;

namespace BingoAPI.Entities.Conditions;

/// <summary>
/// Condition that is valid when the condition is invalid
/// </summary>
internal sealed class NotCondition : BaseCondition
{
    private readonly BaseCondition Condition;

    public NotCondition(JObject json) : base(json)
    {
        var conditions = ParseConditions(json);

        if (conditions.Length < 1)
            throw new ArgumentException("Expected at least one condition.");

        Condition = conditions[0];
    }

    /// <inheritdoc/>
    public override bool Check() => !Condition.Check();
}