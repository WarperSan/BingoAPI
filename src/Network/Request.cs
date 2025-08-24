using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Mime;
using System.Net.WebSockets;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using BingoAPI.Extensions;
using BingoAPI.Helpers;
using Newtonsoft.Json;

namespace BingoAPI.Network;

/// <summary>
/// Class that streamlines the use of web requests
/// </summary>
internal static class Request
{
    private static HttpClient? _client;

    public static void Setup(string baseAddress)
    {
        _client = new HttpClient();

        _client.BaseAddress = new Uri(baseAddress);
        _client.Timeout = new TimeSpan(0, 0, 0, 0, 30_000);
        
        _client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue(MediaTypeNames.Text.Html));
        _client.DefaultRequestHeaders.UserAgent.Add(new ProductInfoHeaderValue(MyPluginInfo.PLUGIN_GUID, MyPluginInfo.PLUGIN_VERSION));
    }
    
    private static async Task<Response> InternalSendRequest(HttpRequestMessage request)
    {
        if (_client == null)
            throw new NullReferenceException("No client was instanced.");
        
        using HttpResponseMessage response = await _client.SendAsync(request);
        
        return new Response
        {
            URL = request.RequestUri.ToString(),
            Code = response.StatusCode,
            Content = await response.Content.ReadAsStringAsync(),
            Error = response.ReasonPhrase,
            IsError = !response.IsSuccessStatusCode
        };
    }

    /// <summary>
    /// Sends a request to the given endpoint with the given method
    /// </summary>
    /// <returns>Response of the request</returns>
    private static Task<Response> SendRequest(string endpoint, HttpMethod method) => InternalSendRequest(new HttpRequestMessage
    {
        RequestUri = new Uri(endpoint, UriKind.Relative),
        Method = method
    });

    /// <summary>
    /// Sends a request to the given endpoint with the given method and the given payload
    /// </summary>
    /// <returns>Response of the request</returns>
    private static Task<Response> SendRequestJSON(string endpoint, HttpMethod method, object payload) => InternalSendRequest(new HttpRequestMessage
    {
        RequestUri = new Uri(endpoint, UriKind.Relative),
        Method = method,
        Content = new StringContent(
            JsonConvert.SerializeObject(payload),
            Encoding.UTF8,
            MediaTypeNames.Application.Json
        )
    });

    /// <summary>
    /// Sends a <c>GET</c> request to the given endpoint
    /// </summary>
    public static Task<Response> Get(string endpoint) => SendRequest(endpoint, HttpMethod.Get);

    /// <summary>
    /// Sends a <c>POST</c> request to the given endpoint with the given payload
    /// </summary>
    public static Task<Response> PostJSON(string endpoint, object payload) => SendRequestJSON(endpoint,  HttpMethod.Post, payload);
    
    /// <summary>
    /// Sends a <c>PUT</c> request to the given endpoint with the given JSON payload
    /// </summary>
    public static Task<Response> PutJSON(string endpoint, object payload) => SendRequestJSON(endpoint,  HttpMethod.Put, payload);
    
    /// <summary>
    /// Sends a <c>POST</c> request to the given endpoint with the given x-www-form-urlencoded payload while using the given CORS token
    /// </summary>
    public static Task<Response> PostCORSForm(string endpoint, string corsToken, object payload)
    {
        var formFields = new Dictionary<string, string>();
        
        foreach (var property in payload.GetType().GetProperties(BindingFlags.Public | BindingFlags.Instance))
            formFields[property.Name] = property.GetValue(payload)?.ToString() ?? "";
        
        var request = new HttpRequestMessage
        {
            RequestUri = new Uri(endpoint),
            Method = HttpMethod.Post,
            Content = new FormUrlEncodedContent(formFields),
        };

        request.Headers.Add("X-CSRFToken", corsToken);

        return InternalSendRequest(request);
    }
    
    /// <summary>
    /// Fetches the hidden CORS token from the given endpoint
    /// </summary>
    public static async Task<string?> GetCORSToken(string endpoint)
    {
        var response = await Get(endpoint);

        if (response.IsError)
        {
            response.PrintError("Failed to fetch CORS token");
            return null;
        }
        
        var match = Regex.Match(
            response.Content,
            "<input[^>]*name=\"csrfmiddlewaretoken\"[^>]*value=\"(.*?)\"[^>]*>"
        );

        if (match.Success)
            return match.Groups[1].Value;

        Log.Error("Could not find the input 'csrfmiddlewaretoken'.");
        return null;
    }
    
    /// <summary>
    /// Creates a socket to the given URI with the given credentials
    /// </summary>
    public static async Task<ClientWebSocket?> CreateSocket(string uri, string? socketKey)
    {
        var socket = new ClientWebSocket();

        try
        {
            // Connect to server
            await socket.ConnectAsync(new Uri(uri), CancellationToken.None);

            // Authenticate to the server
            await socket.SendAsJson(new { socket_key = socketKey });
        }
        catch (Exception e)
        {
            Log.Error($"Error while trying to create a socket with '{socketKey ?? "null"}': {e.Message}");
            socket.Dispose();
            socket = null;
        }

        return socket;
    }
}