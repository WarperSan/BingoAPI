using BingoAPI.Helpers;
using BingoAPI.Network;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

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
            Logger.Warning($"Tried to print the error '{errorMessage}', but the response finished with a success.");
            return;
        }
        
        Logger.Error($"[{response.Code}] {errorMessage}: ({response.Error})");
    }

    /// <summary>
    /// Parses the content of this response to the given type
    /// </summary>
    public static T? Parse<T>(this Response response)
    {
        if (response.IsError)
            return default;

        if (response.Content == null)
            return default;
        
        return JsonConvert.DeserializeObject<T>(response.Content);
    }
    
    /// <summary>
    /// Parses the content of this response into a JSON object
    /// </summary>
    public static JObject? Json(this Response response) => response.Parse<JObject>();
}