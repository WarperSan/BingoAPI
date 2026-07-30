using JetBrains.Annotations;

namespace BingoAPI.Conditions;

/// <summary>
/// Registers any class inheriting <see cref="IConditionFactory"/> to the given action
/// </summary>
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
