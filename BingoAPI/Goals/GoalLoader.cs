using BingoAPI.Conditions;
using BingoAPI.Helpers;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace BingoAPI.Goals;

/// <summary>
/// Loads <see cref="Goal"/> instances from files
/// </summary>
public static class GoalLoader
{
	/// <summary>
	/// Loads all <see cref="Goal"/> from the given JSON file
	/// </summary>
	public static Goal[] Load(string path)
	{
		if (!File.Exists(path))
		{
			Log.Error($"Goal file not found: '{path}'.");
			return [];
		}

		string json;

		try
		{
			json = File.ReadAllText(path);
		}
		catch (Exception e)
		{
			Log.Error($"Failed to read goal file '{path}': {e.Message}");
			return [];
		}

		return Parse(json);
	}

	/// <summary>
	/// Parses all <see cref="Goal"/> from the given JSON string
	/// </summary>
	public static Goal[] Parse(string json)
	{
		JArray root;

		try
		{
			root = JArray.Parse(json);
		}
		catch (JsonException e)
		{
			Log.Error($"Failed to parse goal JSON: {e.Message}");
			return [];
		}

		var goals = new List<Goal>();

		foreach (var token in root)
		{
			if (token is not JObject obj)
				continue;

			var goal = ParseGoal(obj);

			if (goal == null)
				continue;

			goals.Add(goal);
		}

		return goals.ToArray();
	}

	private static Goal? ParseGoal(JObject json)
	{
		try
		{
			var name = json.Value<string>("name");

			if (string.IsNullOrWhiteSpace(name))
				throw new JsonException($"Missing or empty 'name': {json}");

			var conditionJson = json.Value<JObject>("condition");

			if (conditionJson == null)
				throw new JsonException($"Missing 'condition': {json}");

			var condition = ConditionRegistry.ParseCondition(conditionJson);

			return new Goal(name, condition);
		}
		catch (Exception e)
		{
			Log.Error($"Failed to parse goal: {e.Message}");
			return null;
		}
	}
}
