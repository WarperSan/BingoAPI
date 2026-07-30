using Newtonsoft.Json;

namespace BingoAPI.Conditions;

/// <summary>
/// Represents any class that can be used to generate a <see cref="ICondition"/>
/// </summary>
public interface IConditionFactory
{
	/// <summary>
	/// Generates the appropriate <see cref="ICondition"/>
	/// </summary>
	ICondition Generate(JsonReader reader, JsonSerializer serializer);
}
