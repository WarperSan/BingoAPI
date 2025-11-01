using System;

namespace BingoAPI.Entities.Conditions;

/// <summary>
///     Attribute that holds information about <see cref="BaseCondition"/>
/// </summary>
[AttributeUsage(AttributeTargets.Class, Inherited = false)]
// ReSharper disable once ClassNeverInstantiated.Global
public sealed class ConditionAttribute : Attribute
{
    /// <summary>
    ///     Name of the action used by this <see cref="BaseCondition"/>
    /// </summary>
    public readonly string Action;

    public ConditionAttribute(string action)
    {
        Action = action;
    }
}