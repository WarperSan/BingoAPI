# Conditions
## Overview

Conditions are reusable building blocks used to control when a goal is marked or cleared. They can be combined (e.g. with `AND`) to create more complex logic.

> [!IMPORTANT]
> Avoid creating a new condition for a single specific use-case (e.g. one that checks a very particular set of circumstances) unless it truly can't be expressed by composing existing conditions.

## How to add custom conditions

If built-in conditions aren't enough, you can define your own conditions.

To create a new condition, you will need a class that implements the interface `ICondition` and that is marked with the attribute `[Condition]`.

### Get Parameters

It is very easy to access the parameters supplied under `"params"` in the JSON. You only need to create a property using `[JsonProperty]`. The rest will be handled by NewtonSoft, including validation and default values.

### Registering Conditions

Once all types are loaded, the owner will need to call `ConditionRegistry.AddAll()`. This will scan the loaded types and attempt to register the conditions.

### Example

Let's do an example! We want to implement this condition:

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

We will need to create the appropriate condition which will handle it:

```csharp
[Condition("AND")]
internal sealed class AndCondition : ICondition
{
	[JsonProperty("conditions")]
	[JsonRequired]
	public required ICondition[] Conditions { get; init; }

	/// <inheritdoc/>
	public bool IsMet() => _conditions.All(condition => condition.IsMet());
}

// Somewhere in the load
ConditionRegistry.AddAll();
```
