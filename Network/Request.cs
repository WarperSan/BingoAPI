using System.Collections.Generic;
using System.Reflection;
using System.Threading.Tasks;
using BingoAPI.Helpers;
using HtmlAgilityPack;
using Newtonsoft.Json;
using UnityEngine.Networking;

namespace BingoAPI.Network;

internal static class Request
{
    private static async Task Send(UnityWebRequest request)
    {
        var req = request.SendWebRequest();

        while (!req.isDone)
            await Task.Delay(25);
    }

    private static Response CompileResponse(UnityWebRequest req) => new()
    {
        URL = req.url,
        Code = req.responseCode,
        Status = UnityWebRequest.GetHTTPStatusString(req.responseCode),
        
        // Error
        Error = req.error,
        IsError = req.result is not (UnityWebRequest.Result.Success or UnityWebRequest.Result.InProgress),
        
        // Content
        Content = req.downloadHandler.text.Trim()
    };

    public static async Task<Response> Get(string uri)
    {
        using var request = UnityWebRequest.Get(uri);
        request.downloadHandler = new DownloadHandlerBuffer();
        
        await Send(request);
        return CompileResponse(request);
    }

    public static async Task<string?> GetCORSToken(string uri)
    {
        using var request = UnityWebRequest.Get(uri);
        
        request.downloadHandler = new DownloadHandlerBuffer();

        await Send(request);
        var response = CompileResponse(request);

        if (response.IsError)
        {
            response.PrintError("Failed to fetch CORS token");
            return null;
        }

        HtmlDocument doc = new HtmlDocument();
        doc.LoadHtml(response.Content);
        
        var input = doc.DocumentNode.SelectSingleNode("//input[@name='csrfmiddlewaretoken']");
        
        if (input == null)
        {
            Logger.Error("Could not find the input 'csrfmiddlewaretoken'.");
            return null;
        }

        var token = input.GetAttributeValue("value", null);
        
        if (token == null)
        {
            Logger.Error("Could not find the attribute 'value'.");
            return null;
        }
        
        return token;
    }

    public static async Task<Response> PostJson(string uri, object payload)
    {
        var json = JsonConvert.SerializeObject(payload);
        using var request = UnityWebRequest.Post(uri, json, "application/json");
        request.downloadHandler = new DownloadHandlerBuffer();

        await Send(request);
        return CompileResponse(request);
    }

    public static async Task<Response> PostCORSForm(string uri, string corsToken, object payload)
    {
        var formFields = new Dictionary<string, string>();
        
        foreach (var property in payload.GetType().GetProperties(BindingFlags.Public | BindingFlags.Instance))
            formFields[property.Name] = property.GetValue(payload).ToString();
        
        using var request = UnityWebRequest.Post(uri, formFields);
        request.downloadHandler = new DownloadHandlerBuffer();
        
        request.SetRequestHeader("X-CSRFToken", corsToken);
        
        await Send(request);
        return CompileResponse(request);
    }

    public static async Task<Response> PutJson(string uri, object payload)
    {
        var json = JsonConvert.SerializeObject(payload);
        using var request = UnityWebRequest.Put(uri, json);
        request.downloadHandler = new DownloadHandlerBuffer();

        await Send(request);
        return CompileResponse(request);
    }
}