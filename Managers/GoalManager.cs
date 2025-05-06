using System.Collections.Generic;
using BingoAPI.Helpers;
using BingoAPI.Models;

namespace BingoAPI.Managers;

public class GoalManager
{
    private static readonly Dictionary<string, Goal> registeredGoals = [];
    
    /// <summary>
    /// Number of goal that are active
    /// </summary>
    public static int ActiveGoalCount { get; private set; }
    
    /// <summary>
    /// Registers the given goal
    /// </summary>
    /// <returns>Success of the registering</returns>
    public static bool RegisterGoal(string guid, string title)
    {
        if (registeredGoals.ContainsKey(guid))
        {
            Logger.Error($"There is already a goal registered under the GUID '{guid}'.");
            return false;
        }

        var goal = new Goal
        {
            GUID = guid,
            IsActive = true,
            Title = title
        };
        
        registeredGoals.Add(guid, goal);
        ActiveGoalCount++;
        return true;
    }

    /// <summary>
    /// Changes the activeness of the goal registered under the given GUID
    /// </summary>
    public static void SetActiveGoal(string guid, bool isActive)
    {
        if (!registeredGoals.TryGetValue(guid, out var goal))
        {
            Logger.Error($"No goal has been registered under the GUID '{guid}'.");
            return;
        }
        
        if (goal.IsActive == isActive)
            return;

        if (isActive)
            ActiveGoalCount++;
        else
            ActiveGoalCount--;

        goal.IsActive = isActive;
        registeredGoals[guid] = goal;
    }

    /// <summary>
    /// Fetches all registered goals
    /// </summary>
    public static List<Goal> GetAllGoals()
    {
        var goals = new List<Goal>();

        foreach (var (_, goal) in registeredGoals)
            goals.Add(goal);

        return goals;
    }
}