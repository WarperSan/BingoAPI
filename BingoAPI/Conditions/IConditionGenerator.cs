namespace BingoAPI.Conditions;

/// <summary>
/// Represents any class that can be used to generate a <see cref="ICondition"/>
/// </summary>
public interface IConditionGenerator
{
	/// <summary>
	/// Generates the appropriate <see cref="ICondition"/>
	/// </summary>
	ICondition Generate();
}
