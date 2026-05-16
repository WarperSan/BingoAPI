using BingoAPI.Helpers;
using Newtonsoft.Json;

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
		var goals = JsonConvert.DeserializeObject<Goal[]>(json);

		if (goals == null)
		{
			Log.Error($"Failed to parse goal JSON: {json}");
			return [];
		}

		return goals;
	}
}
