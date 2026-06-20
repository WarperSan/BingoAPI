# Conditions
## Overview

Conditions are reusable building blocks used to control when a goal is marked or cleared. They can be combined (e.g. with `AND`) to create more complex logic.

> [!IMPORTANT]
> Avoid creating a new condition for a single specific use-case (e.g. one that checks a very particular set of circumstances) unless it truly can't be expressed by composing existing conditions.

## How to add custom conditions

If built-in conditions aren't enough, you can define your own conditions.

Here are the things required to implement a new condition:
1. A class that implements the interface `ICondition`
2. A method marked with the attribute `[Condition]` that receives a `ConditionData` and returns an `ICondition`

### Get the Parameters

The `ConditionData` object gives you access to the parameters supplied under `"params"` in the JSON.

It exposes `GetRequiredParameter<T>(...)` and `GetOptionalParameter<T>(...)` to read those parameters. These methods will automatically manage missing values and type casting.

Parameters are deserialized using standard JSON deserialization. This allows converters such as `StringEnumConverter` work as expected.

> [!IMPORTANT]
> The key used in methods like `GetRequiredParameter()` is the **exact** key that must be present in the JSON.

### Register the Condition

There are two ways to register a condition:
- Using the attribute `[Condition("NAME")]`
- Using `ConditionRegistry.TryAdd(...)`

The attribute is the recommended approach for most conditions: it registers the condition automatically and keeps the registration alongside the implementation, in the same class.

However, there are cases where extra logic should occur (e.g. reusing conditions, creating the correct condition on a given parameter). To do so, you will need to run `ConditionRegistry.TryAdd()` while passing the action key and the method to call.

### Example

The following example shows how to implement this condition using both methods:

```json
{
	"action": "AND",
	"params": {
		"conditions": [
			{ ... },
			{ ... }
		]
	}
}
```

#### Using `[Condition]`
```csharp
internal sealed class AndCondition : ICondition
{
	private readonly ICondition[] _conditions;

	[Condition("AND")]
	public AndCondition(ConditionData data)
	{
		_conditions = data.GetRequiredParameter<ICondition[]>("conditions");
	}

	/// <inheritdoc/>
	public bool IsMet() => _conditions.All(condition => condition.IsMet());
}
```

#### Using `ConditionRegistry.TryAdd()`

```csharp
internal sealed class AndCondition : ICondition
{
	private readonly ICondition[] _conditions;

	private AndCondition(ICondition[] conditions)
	{
		_conditions = conditions;
	}

	// This can be in another class if necessary
	public static ICondition Create(ConditionData data)
	{
		var conditions = data.GetRequiredParameter<ICondition[]>("conditions");

		if (conditions.Length == 1)
			return conditions[0];

		return new AndCondition(conditions);
	}

	/// <inheritdoc/>
	public bool IsMet() => _conditions.All(condition => condition.IsMet());
}

// Somewhere in the load
ConditionRegistry.TryAdd("AND", AndCondition.Create);
```
