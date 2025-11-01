using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;

namespace BingoAPI.Network;

/// <summary>
/// Wrapper that exposes every useful information about a response
/// </summary>
internal record Response
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
    /// Headers returned by the server
    /// </summary>
    public readonly HttpResponseHeaders Headers;

    /// <summary>
    /// Text returned as the response's body data
    /// </summary>
    public readonly string Content;

    /// <summary>
    /// Human-readable string describing any system errors encountered
    /// </summary>
    public readonly string Error;

    /// <summary>
    /// Computes the error status of the response
    /// </summary>
    public readonly bool IsError;

    private Response(HttpRequestMessage request, HttpResponseMessage response, string responseContent)
    {
        URL = request.RequestUri.ToString();
        Code = response.StatusCode;
        Headers = response.Headers;
        Content = responseContent;
        Error = response.ReasonPhrase;
        IsError = !response.IsSuccessStatusCode;
    }

    public static async Task<Response> Create(HttpRequestMessage request, HttpResponseMessage response) => new(
        request,
        response,
        await response.Content.ReadAsStringAsync()
    );
}