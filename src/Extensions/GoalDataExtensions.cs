using System.Text;
using BingoAPI.Models;

namespace BingoAPI.Extensions;

/// <summary>
/// Provides extension methods for <see cref="GoalData"/>
/// </summary>
internal static class GoalDataExtensions
{
    /// <summary>
    /// Generates the JSON for the given goals
    /// </summary>
    /// <returns></returns>
    public static string GenerateJSON(this GoalData[] goals)
    {
        var builder = new StringBuilder();

        builder.Append("[");

        for (var i = 0; i < goals.Length; i++)
        {
            var goal = goals[i];
            
            builder.Append("{");
            
            builder.Append($"\"name\":\"{goal.Name}\"");

            builder.Append("}");

            if (i != goals.Length - 1)
                builder.Append(",");
        }
        
        builder.Append("]");

        return builder.ToString();
    }
}