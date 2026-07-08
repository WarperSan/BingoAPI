namespace BingoAPI.Conditions;

/// <summary>
/// Defines any method that can make a <see cref="ICondition"/> from a given <see cref="ConditionData"/>
/// </summary>
[AttributeUsage(AttributeTargets.Class)]
public class ConditionAttribute : Attribute
{
	/// <summary>
	/// Action key to add this factory under
	/// </summary>
	public readonly string Action;

	/// <summary>
	/// Defines this method as a valid factory for <see cref="ICondition"/>
	/// </summary>
	/// <param name="action">
	///	<inheritdoc cref="Action"/>
	/// </param>
	public ConditionAttribute(string action)
	{
		Action = action;
	}
}
