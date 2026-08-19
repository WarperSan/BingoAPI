using JetBrains.Annotations;
using Newtonsoft.Json;

namespace BingoAPI.Conditions;

/// <summary>
/// Represents any class that can be used to generate a <see cref="ICondition"/>
/// </summary>
[PublicAPI]
public interface IConditionFactory
{
	/// <summary>
	/// Generates a <see cref="ICondition"/>
	/// </summary>
	public ICondition Generate(JsonReader reader, JsonSerializer serializer);
}
