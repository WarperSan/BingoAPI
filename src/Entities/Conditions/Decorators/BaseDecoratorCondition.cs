using System;
using Newtonsoft.Json.Linq;

namespace BingoAPI.Entities.Conditions.Decorators;

/// <summary>
/// Class that represents any condition with one child attached to it
/// </summary>
public abstract class BaseDecoratorCondition : BaseCondition
{
    /// <summary>
    /// Condition attached to this one
    /// </summary>
    protected readonly BaseCondition Condition;

    /// <inheritdoc/>
    protected BaseDecoratorCondition(JObject json) : base(json)
    {
        var conditions = ParseConditions(json);

        if (conditions.Length < 1)
            throw new ArgumentException("Expected at least one condition.");

        Condition = conditions[0];
    }
}