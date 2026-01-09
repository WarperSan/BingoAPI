using System;
using System.Collections.Generic;
using System.Net;
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

    public static void Setup()
    {
        _client = new HttpClient();

        _client.BaseAddress = new Uri("https://bingosync.com");
        _client.Timeout = TimeSpan.FromMilliseconds(30_000);

        _client.DefaultRequestHeaders.UserAgent.Add(new ProductInfoHeaderValue(MyPluginInfo.PLUGIN_GUID, MyPluginInfo.PLUGIN_VERSION));
    }

    #region Send

    /// <summary>
    /// Sends the given request
    /// </summary>
    /// <returns>Compiled response</returns>
    private static async Task<Response> SendRequest(HttpRequestMessage request)
    {
        if (_client == null)
            throw new NullReferenceException("No client was instanced.");

        var requestContent = request.Content != null ? await request.Content.ReadAsStringAsync() : "";

        var responseMessage = await _client.SendAsync(request);

        var response = await Response.Create(request, responseMessage);

        Log.Debug($"{request.Method} {request.RequestUri}\n{request.Headers}\n{requestContent}");
        Log.Debug($"{response.Code} {response.URL}\n{response.Headers}\n{response.Content}");

        request.Dispose();
        responseMessage.Dispose();

        return response;
    }

    /// <summary>
    /// Sends a request to the given endpoint with the given method
    /// </summary>
    /// <returns>Response of the request</returns>
    private static Task<Response> SendRequest(string endpoint, HttpMethod method) => SendRequest(new HttpRequestMessage
    {
        RequestUri = new Uri(endpoint, UriKind.Relative),
        Method = method
    });

    /// <summary>
    /// Sends a request to the given endpoint with the given method and the given payload
    /// </summary>
    /// <returns>Response of the request</returns>
    private static Task<Response> SendRequestJson(string endpoint, HttpMethod method, object payload) => SendRequest(new HttpRequestMessage
    {
        RequestUri = new Uri(endpoint, UriKind.Relative),
        Method = method,
        Content = new StringContent(
            JsonConvert.SerializeObject(payload),
            Encoding.UTF8,
            "application/json"
        )
    });

    #endregion

    #region REST

    /// <summary>
    /// Sends a <c>GET</c> request to the given endpoint
    /// </summary>
    public static Task<Response> Get(string endpoint) => SendRequest(endpoint, HttpMethod.Get);

    /// <summary>
    /// Sends a <c>POST</c> request to the given endpoint with the given payload
    /// </summary>
    public static Task<Response> PostJson(string endpoint, object payload) => SendRequestJson(endpoint, HttpMethod.Post, payload);

    /// <summary>
    /// Sends a <c>PUT</c> request to the given endpoint with the given JSON payload
    /// </summary>
    public static Task<Response> PutJson(string endpoint, object payload) => SendRequestJson(endpoint, HttpMethod.Put, payload);

    /// <summary>
    /// Sends a <c>POST</c> request to the given endpoint with the given x-www-form-urlencoded payload while using the given CORS tokens
    /// </summary>
    public static Task<Response> PostCorsForm(
        string endpoint,
        string publicToken,
        string hiddenToken,
        object payload
    )
    {
        var formFields = new Dictionary<string, string>();

        foreach (var property in payload.GetType().GetProperties(BindingFlags.Public | BindingFlags.Instance))
            formFields[property.Name] = property.GetValue(payload)?.ToString() ?? "";

        var request = new HttpRequestMessage
        {
            RequestUri = new Uri(endpoint, UriKind.Relative),
            Method = HttpMethod.Post,
            Content = new FormUrlEncodedContent(formFields)
        };

        request.Headers.Add("Cookie", $"csrftoken={publicToken}");
        request.Headers.Add("X-CSRFToken", hiddenToken);

        return SendRequest(request);
    }

    /// <summary>
    /// Fetches the public and hidden CORS token from the given endpoint
    /// </summary>
    public static async Task<(string _public, string _hidden)?> GetCorsTokens(string endpoint)
    {
        var response = await Get(endpoint);

        if (response.IsError)
        {
            response.PrintError("Failed to fetch CORS tokens");
            return null;
        }

        if (_client == null)
        {
            Log.Error("The client was destroyed before processing the CORS tokens.");
            return null;
        }

        if (!response.Headers.TryGetValues("Set-Cookie", out var setCookie))
        {
            Log.Error("No cookie was set.");
            return null;
        }

        var container = new CookieContainer();

        foreach (var cookieHeader in setCookie)
            container.SetCookies(_client.BaseAddress, cookieHeader);

        var cookies = container.GetCookies(_client.BaseAddress);
        var publicTokenCookie = cookies["csrftoken"];

        if (publicTokenCookie == null)
        {
            Log.Error("Could not find the public CORS token.");
            return null;
        }

        var match = Regex.Match(
            response.Content,
            "<input[^>]*name=\"csrfmiddlewaretoken\"[^>]*value=\"(.*?)\"[^>]*>"
        );

        if (!match.Success)
        {
            Log.Error("Could not find the hidden CORS token.");
            return null;
        }

        return (publicTokenCookie.Value, match.Groups[1].Value);
    }

    #endregion

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