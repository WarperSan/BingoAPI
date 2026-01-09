using Newtonsoft.Json.Linq;

namespace BingoAPI.Entities.Conditions.Builtin;

/// <summary>
/// Condition that is valid when the condition is invalid
/// </summary>
[Condition("NOT")]
internal sealed class NotCondition : BaseCondition
{
    private readonly BaseCondition? _condition;

    public NotCondition(JObject json) : base(json)
    {
        var rawCondition = json.Value<JObject>("condition");

        if (rawCondition == null)
            throw new ArgumentException($"Expected 'condition': {json}");

        _condition = ParseCondition(rawCondition);
    }

    /// <inheritdoc/>
    public override bool Check() => !_condition?.Check() ?? false;
}