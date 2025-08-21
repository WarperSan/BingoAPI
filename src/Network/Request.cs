using System;
using System.Collections.Generic;
using System.Net.WebSockets;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using BingoAPI.Configurations;
using BingoAPI.Extensions;
using Newtonsoft.Json;
using UnityEngine.Networking;
using Logger = BingoAPI.Helpers.Logger;

namespace BingoAPI.Network;

/// <summary>
/// Class that streamlines the use of web requests
/// </summary>
internal static class Request
{
    /// <summary>
    /// Periodically checks if the callback is done or if it has timed out
    /// </summary>
    internal static async Task<bool> HandleTimeout(Func<bool> callback)
    {
        var delay = Configuration.Instance?.Network.NetworkDelayMS.Value ?? 25;
        var timeout = Configuration.Instance?.Network.NetworkTimeoutMS.Value ?? 30_000;
        
        while (!callback.Invoke() && timeout > 0)
        {
            await Task.Delay(delay);
            timeout -= delay;
        }

        return timeout > 0;
    }
    
    private static async Task<Response> InternalSendRequest(UnityWebRequest request)
    {
        request.SetRequestHeader("User-Agent", $"{PluginInfo.PLUGIN_GUID}/{PluginInfo.PLUGIN_VERSION}");

        var requestOperation = request.SendWebRequest();
        await HandleTimeout(() => requestOperation.isDone);
        
        var response = new Response(request);
        
        request.Dispose();
        
        return response;
    }

    /// <summary>
    /// Sends a request to the given URI with the given method
    /// </summary>
    /// <returns>Response of the request</returns>
    private static Task<Response> SendRequest(string uri, string method, UploadHandler? uploadHandler) => InternalSendRequest(
        new UnityWebRequest(
            uri,
            method,
            new DownloadHandlerBuffer(),
            uploadHandler
        )
    );

    /// <summary>
    /// Sends a request to the given URI with the given method and the given payload
    /// </summary>
    /// <returns>Response of the request</returns>
    private static Task<Response> SendRequestJSON(string uri, string method, object payload)
    {
        var json = JsonConvert.SerializeObject(payload);
        var bytes = Encoding.UTF8.GetBytes(json);
        var uploadHandler = new UploadHandlerRaw(bytes);
        uploadHandler.contentType = "application/json";
        
        return SendRequest(uri, method, uploadHandler);
    }

    /// <summary>
    /// Sends a <c>GET</c> request to the given URI
    /// </summary>
    public static Task<Response> Get(string uri) => SendRequest(uri, UnityWebRequest.kHttpVerbGET, null);

    /// <summary>
    /// Sends a <c>POST</c> request to the given URI with the given payload
    /// </summary>
    public static Task<Response> PostJSON(string uri, object payload) => SendRequestJSON(uri,  UnityWebRequest.kHttpVerbPOST, payload);
    
    /// <summary>
    /// Sends a <c>PUT</c> request to the given URI with the given JSON payload
    /// </summary>
    public static Task<Response> PutJSON(string uri, object payload) => SendRequestJSON(uri,  UnityWebRequest.kHttpVerbPUT, payload);
    
    /// <summary>
    /// Sends a <c>POST</c> request to the given URI with the given x-www-form-urlencoded payload while using the given CORS token
    /// </summary>
    public static Task<Response> PostCORSForm(string uri, string corsToken, object payload)
    {
        var formFields = new Dictionary<string, string>();
        
        foreach (var property in payload.GetType().GetProperties(BindingFlags.Public | BindingFlags.Instance))
            formFields[property.Name] = property.GetValue(payload)?.ToString() ?? "";

        var data = UnityWebRequest.SerializeSimpleForm(formFields);
        var uploadHandler = new UploadHandlerRaw(data);
        uploadHandler.contentType = "application/x-www-form-urlencoded";
        
        var request = new UnityWebRequest(
            uri,
            UnityWebRequest.kHttpVerbPOST,
            new DownloadHandlerBuffer(),
            uploadHandler
        );
        
        request.SetRequestHeader("X-CSRFToken", corsToken);
        
        return InternalSendRequest(request);
    }
    
    /// <summary>
    /// Fetches the hidden CORS token from the given URI
    /// </summary>
    public static async Task<string?> GetCORSToken(string uri)
    {
        var response = await Get(uri);

        if (response.IsError)
        {
            response.PrintError("Failed to fetch CORS token");
            return null;
        }
        
        var match = Regex.Match(
            response.Content ?? "",
            "<input[^>]*name=\"csrfmiddlewaretoken\"[^>]*value=\"(.*?)\"[^>]*>"
        );

        if (match.Success)
            return match.Groups[1].Value;

        Logger.Error("Could not find the input 'csrfmiddlewaretoken'.");
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
            Logger.Error($"Error while trying to create a socket with '{socketKey ?? "null"}': {e.Message}");
            socket.Dispose();
            socket = null;
        }

        return socket;
    }
}