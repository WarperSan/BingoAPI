using System;
using System.Collections.Generic;
using System.IO;
using BingoAPI.Entities.Conditions;
using BingoAPI.Helpers;
using BingoAPI.Models;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace BingoAPI.Managers;

/// <summary>
/// Class that offers static calling for managing <see cref="Goal"/>
/// </summary>
public static class GoalManager
{
    /// <summary>
    /// Parses the JSON at the given file into a list of goals
    /// </summary>
    public static Goal[] Parse(string path)
    {
        if (!File.Exists(path))
            return [];

        var content = File.ReadAllText(path);
        var rawGoals = JsonConvert.DeserializeObject<JArray>(content);

        if (rawGoals == null)
            return [];

        var goals = new List<Goal>();

        for (int i = 0; i < rawGoals.Count; i++)
        {
            var rawGoal = rawGoals[i];

            var name = rawGoal.Value<string>("name");
            var rawCondition = rawGoal.Value<JObject>("condition");

            if (name == null || rawCondition == null)
            {
                Log.Warning($"Skipping an invalid goal: {rawGoal}");
                continue;
            }

            var condition = BaseCondition.ParseCondition(rawCondition);

            if (condition == null)
            {
                Log.Warning($"Skipping an invalid condition: {rawCondition}");
                continue;
            }

            goals.Add(new Goal
            {
                Name = name,
                Condition = condition
            });
        }

        return goals.ToArray();
    }

    public static void AddGoals(Goal[] goals)
    {
        throw new NotImplementedException();
    }

    public static bool AddGoal(string uuid, Goal goal)
    {
        throw new NotImplementedException();
    }
    
    /// <summary>
    /// Gets all the <see cref="Goal"/> registered
    /// </summary>
    public static Goal[] GetAll()
    {
        throw new NotImplementedException();
    }

    /// <summary>
    /// Gets the registered <see cref="Goal"/> pointed to the given data
    /// </summary>
    public static Goal? GetGoal(SquareData data)
    {
        throw new NotImplementedException();
    }
}