using BingoAPI.Conditions.Interfaces;
using JetBrains.Annotations;
using Newtonsoft.Json;

namespace BingoAPI.Conditions.Factories;

/// <summary>
/// Factory used to generate <see cref="ICondition"/> at runtime using the given <typeparamref name="TParams"/>
/// </summary>
[PublicAPI]
public abstract class ParameterizedConditionFactory<TParams> : IConditionFactory
{
	/// <inheritdoc />
	public ICondition Generate(JsonReader reader, JsonSerializer serializer)
	{
		var parameters = serializer.Deserialize<TParams>(reader);

		if (parameters == null)
			throw new JsonException(
				$"Failed to deserialize the parameters as '{typeof(TParams)}'."
			);

		return Generate(parameters);
	}

	/// <summary>
	/// Generates a <see cref="ICondition"/> using the given <typeparamref name="TParams"/>
	/// </summary>
	protected abstract ICondition Generate(TParams parameters);
}
