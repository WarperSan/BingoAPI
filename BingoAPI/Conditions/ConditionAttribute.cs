using JetBrains.Annotations;

namespace BingoAPI.Conditions;

/// <summary>
/// Registers any class inheriting <see cref="ICondition"/> to the given action
/// </summary>
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
