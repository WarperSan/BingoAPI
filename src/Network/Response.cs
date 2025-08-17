using UnityEngine.Networking;

namespace BingoAPI.Network;

/// <summary>
/// Structure that holds every useful information about a response
/// </summary>
internal readonly struct Response
{
    /// <summary>
    /// Target URL for the request to communicate with
    /// </summary>
    public readonly string URL;
    
    /// <summary>
    /// Numeric HTTP response code returned by the server
    /// </summary>
    public readonly long Code;
    
    /// <summary>
    /// Human-readable string describing any system errors encountered
    /// </summary>
    public readonly string Error;

    /// <summary>
    /// Computes the error status of the response
    /// </summary>
    public readonly bool IsError;

    /// <summary>
    /// Text returned as the response's body data
    /// </summary>
    public readonly string? Content;
    
    public Response(UnityWebRequest req)
    {
        URL = req.url;
        Code = req.responseCode;
        
        // Error
        Error = req.error;
        IsError = req.result == UnityWebRequest.Result.ProtocolError;
        
        // Content
        Content = req.downloadHandler?.text.Trim();
    }
}