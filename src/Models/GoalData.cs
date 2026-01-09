using BingoAPI.Entities.Conditions;
using BingoAPI.Helpers;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace BingoAPI.Models;

/// <summary>
/// Data for a goal
/// </summary>
public readonly struct GoalData
{
	/// <summary>
	/// Text to show when displaying this goal
	/// </summary>
	public readonly string Name;

	/// <summary>
	/// Condition to fulfill in order to mark this goal
	/// </summary>
	public readonly BaseCondition Condition;

	internal GoalData(JToken? obj)
	{
		var name = obj?.Value<string>("name");
		var rawCondition = obj?.Value<JObject>("condition");

		if (rawCondition == null)
			throw new ArgumentException("Expected a condition for the goal.");

		Name = name ?? throw new ArgumentException("Expected a name for the goal.");
		Condition = BaseCondition.ParseCondition(rawCondition) ?? throw new NullReferenceException("Could not parse the condition.");
	}

	/// <summary>
	/// Loads the list of goals from the given file
	/// </summary>
	public static GoalData[] LoadFromFile(string file)
	{
		if (!File.Exists(file))
		{
			Log.Error($"Could not find the file at '{file}'.");
			return [];
		}

		var fileContent = File.ReadAllText(file);
		var json = JsonConvert.DeserializeObject<JArray>(fileContent);

		if (json == null)
		{
			Log.Error($"Failed to create goals from '{file}'.");
			return [];
		}

		var goals = new List<GoalData>();

		foreach (var child in json)
		{
			try
			{
				goals.Add(new GoalData(child));
			}
			catch (Exception e)
			{
				Log.Warning(e.Message);
			}
		}

		return goals.ToArray();
	}
}
