using System.Net;
using System.Net.Http.Headers;

namespace BingoAPI.Network;

/// <summary>
/// Structure that holds every useful information about a response
/// </summary>
internal struct Response
{
    /// <summary>
    /// Target URL for the request to communicate with
    /// </summary>
    public string URL;
    
    /// <summary>
    /// Numeric HTTP response code returned by the server
    /// </summary>
    public HttpStatusCode Code;
    
    /// <summary>
    /// Headers returned by the server
    /// </summary>
    public HttpResponseHeaders Headers;
    
    /// <summary>
    /// Text returned as the response's body data
    /// </summary>
    public string Content;
    
    /// <summary>
    /// Human-readable string describing any system errors encountered
    /// </summary>
    public string Error;

    /// <summary>
    /// Computes the error status of the response
    /// </summary>
    public bool IsError;
}