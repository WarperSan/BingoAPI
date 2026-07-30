using BingoAPI.Conditions.Interfaces;
using Newtonsoft.Json;

namespace BingoAPI.Conditions.Factories;

/// <summary>
/// Factory used to generate a <see cref="ICondition"/> using JSON
/// </summary>
internal sealed class ConditionJsonFactory : IConditionFactory
{
	private readonly Type _type;

	/// <summary>
	/// Initializes a new instance of the <see cref="ConditionJsonFactory"/> class.
	/// </summary>
	public ConditionJsonFactory(Type type)
	{
		_type = type;
	}

	/// <inheritdoc />
	public ICondition Generate(JsonReader reader, JsonSerializer serializer)
	{
		var instance = Activator.CreateInstance(_type);

		if (instance is not ICondition condition)
			throw new ArgumentException(
				$"Type '{_type}' cannot be used inside '{nameof(ConditionJsonFactory)}'."
			);

		serializer.Populate(reader, condition);

		return condition;
	}
}
