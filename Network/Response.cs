using BingoAPI.Helpers;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace BingoAPI.Network;

/// <summary>
/// Structure that holds every useful information of a response
/// </summary>
internal struct Response
{
    public string URL;
    public long Code;
    public string Status;
    
    // Error
    public string Error;
    public bool IsError;
    public void PrintError(string errorMessage) => Logger.Error($"[{Code}] {errorMessage}: {Status} ({Error})");
    
    // Content
    public string Content;
    public T? Parse<T>() => JsonConvert.DeserializeObject<T>(Content);
    public JObject? Json() => Parse<JObject>();
}