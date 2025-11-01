using BingoAPI.Helpers;
using BingoAPI.Network;
using Newtonsoft.Json;

namespace BingoAPI.Extensions;

/// <summary>
/// Provides extension methods for <see cref="Response"/>
/// </summary>
internal static class ResponseExtensions
{
    /// <summary>
    /// Prints the error of this response with the given message
    /// </summary>
    public static void PrintError(this Response response, string errorMessage)
    {
        if (!response.IsError)
        {
            Log.Warning($"Tried to print the error '{errorMessage}', but the response finished with a success.");
            return;
        }

        Log.Error($"[{response.Code}] {errorMessage}: {response.Error}");
    }

    /// <summary>
    /// Parses the content of this response to the given type
    /// </summary>
    public static T? ParseJson<T>(this Response response) => response.IsError ? default : JsonConvert.DeserializeObject<T>(response.Content);
}