using Newtonsoft.Json;

namespace BingoAPI.Conditions.Interfaces;

/// <summary>
/// Represents any class that can be used to generate a <see cref="ICondition"/>
/// </summary>
public interface IConditionFactory
{
	/// <summary>
	/// Generates a <see cref="ICondition"/>
	/// </summary>
	ICondition Generate(JsonReader reader, JsonSerializer serializer);
}
