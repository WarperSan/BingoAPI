using System;
using System.Net.WebSockets;
using System.Threading.Tasks;
using UnityEngine.Networking;

namespace BingoAPI.Extensions;

internal static class RequestExtension
{
    /// <summary>
    /// Handles the timeout for the given request
    /// </summary>
    public static Task<bool> HandleTimeout(this UnityWebRequestAsyncOperation request)
        => HandleTimeout(() => request.isDone);
    
    /// <summary>
    /// Handles the timeout for the given socket
    /// </summary>
    public static Task<bool> HandleTimeout(this ClientWebSocket socket)
        => HandleTimeout(() => socket.State == WebSocketState.Open);
    
    internal static async Task<bool> HandleTimeout(Func<bool> callback)
    {
        var delay = Plugin.Instance?.configNetworkDelayMS?.Value ?? 25;
        var timeout = Plugin.Instance?.configNetworkTimeoutMS?.Value ?? 30_000;
        
        while (!callback.Invoke() && timeout > 0)
        {
            await Task.Delay(delay);
            timeout -= delay;
        }

        return timeout > 0;
    }
}