using System.Text;
using BingoAPI.Models;

namespace BingoAPI.Extensions;

public static class GoalExtension
{
    /// <summary>
    /// Generates the JSON from the given goals
    /// </summary>
    /// <returns></returns>
    internal static string GenerateJSON(this Goal[] goals)
    {
        var builder = new StringBuilder();

        builder.Append("[");

        for (int i = 0; i < goals.Length; i++)
        {
            var goal = goals[i];
            
            builder.Append("{");
            
            builder.Append($"\"name\":\"{goal.Title}\"");

            builder.Append("}");

            if (i != goals.Length - 1)
                builder.Append(",");
        }
        
        builder.Append("]");

        return builder.ToString();
    }
}