using Newtonsoft.Json.Linq;

namespace BingoAPI.Entities.Conditions.Decorators;

/// <summary>
/// Condition that is valid when the condition is invalid
/// </summary>
internal sealed class NotCondition : BaseDecoratorCondition
{
    public NotCondition(JObject json) : base(json)
    {
    }

    /// <inheritdoc/>
    public override bool Check() => !Condition.Check();
}