using JetBrains.Annotations;

namespace BingoAPI.Conditions.Attributes;

/// <summary>
/// Registers any class inheriting <see cref="ICondition"/> to the given action
/// </summary>
/// <remarks>
/// Use this attribute only if the condition must be registered from
/// <see cref="ConditionRegistry.TryRegisterCondition(Type)"/>. Otherwise,
/// use <see cref="ConditionRegistry.RegisterCondition{T}(string)"/>
/// </remarks>
[AttributeUsage(AttributeTargets.Class)]
[PublicAPI]
public class ConditionAttribute : Attribute
{
	/// <summary>
	/// Action key to add this condition under
	/// </summary>
	public readonly string Action;

	/// <summary>
	/// Initializes a new instance of the <see cref="ConditionAttribute"/> class.
	/// </summary>
	public ConditionAttribute(string action)
	{
		Action = action;
	}
}
