namespace BingoAPI.Conditions;

/// <summary>
/// Represents any class that can be used as a condition
/// </summary>
public interface ICondition
{
	/// <summary>
	/// Checks if this condition is met
	/// </summary>
	bool Check();
}
