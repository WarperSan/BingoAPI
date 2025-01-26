using System;
using System.Net.WebSockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using BingoAPI.Helpers;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace BingoAPI.Network;

internal static class Socket
{
    /// <summary>
    /// Sends a request using the given payload at the given socket
    /// </summary>
    public static async Task SendAsJson(this ClientWebSocket socket, object value)
    {
        string jsonMessage = JsonConvert.SerializeObject(value);
        byte[] buffer = Encoding.UTF8.GetBytes(jsonMessage);
        await socket.SendAsync(new ArraySegment<byte>(buffer), WebSocketMessageType.Text, true, CancellationToken.None);
    }
    
    /// <summary>
    /// Creates a socket with the given key
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
    
    public static async Task HandleMessages(this ClientWebSocket socket, Action<JObject?> onReceive)
    {
        byte[] buffer = new byte[1024];

        while (socket.State == WebSocketState.Open)
        {
            WebSocketReceiveResult result = await socket.ReceiveAsync(new ArraySegment<byte>(buffer), CancellationToken.None);
            var message = Encoding.UTF8.GetString(buffer, 0, result.Count);
            var json = JsonConvert.DeserializeObject<JObject>(message);
            
            onReceive?.Invoke(json);
        }
    }
}