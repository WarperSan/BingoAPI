using JetBrains.Annotations;

namespace BingoAPI.Conditions.Attributes;

/// <summary>
/// Registers any class inheriting <see cref="IConditionFactory"/> to the given action
/// </summary>
/// <remarks>
/// Use this attribute only if the factory must be registered from
/// <see cref="ConditionRegistry.TryRegisterFactory(Type)"/>. Otherwise,
/// use <see cref="ConditionRegistry.RegisterFactory(string,IConditionFactory)"/>
/// </remarks>
[AttributeUsage(AttributeTargets.Class)]
[PublicAPI]
public class ConditionFactoryAttribute : Attribute
{
	/// <summary>
	/// Action key to add this factory under
	/// </summary>
	public readonly string Action;

	/// <summary>
	/// Initializes a new instance of the <see cref="ConditionFactoryAttribute"/> class.
	/// </summary>
	public ConditionFactoryAttribute(string action)
	{
		Action = action;
	}
}
