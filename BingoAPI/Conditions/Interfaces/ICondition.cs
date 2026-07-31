using Newtonsoft.Json;

namespace BingoAPI.Conditions.Interfaces;

/// <summary>
/// Represents any class that can be used as a condition
/// </summary>
[JsonConverter(typeof(ConditionConverter))]
public interface ICondition
{
	/// <summary>
	/// Checks if this condition is met
	/// </summary>
	bool IsMet();
}
