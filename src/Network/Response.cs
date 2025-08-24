using System.Net;
using System.Net.Http;
using BingoAPI.Helpers;

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
    public readonly HttpStatusCode Code;
    
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
    public readonly string Content;
    
    public Response(HttpResponseMessage response, string content)
    {
        var request = response.RequestMessage;
        
        URL = request.RequestUri.AbsoluteUri;
        Code = response.StatusCode;
        
        // Error
        Error = response.ReasonPhrase;
        IsError = !response.IsSuccessStatusCode;
        
        // Content
        Content = content;

        Log.Debug($"{request.Method.Method} {request.RequestUri} {request.Version}\n{request.Headers}\n\n{request.Content}");
    }
}