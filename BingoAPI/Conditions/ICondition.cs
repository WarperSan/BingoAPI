namespace BingoAPI.Conditions;

public interface ICondition
{
	/// <summary>
	/// Checks if this condition is met
	/// </summary>
	bool Check();
}
