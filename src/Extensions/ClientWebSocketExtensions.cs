using System;
using System.Net.WebSockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using BingoAPI.Helpers;
using Newtonsoft.Json;

namespace BingoAPI.Extensions;

/// <summary>
/// Provides extension methods for <see cref="ClientWebSocket"/>
/// </summary>
internal static class ClientWebSocketExtensions
{
    /// <summary>
    /// Sends the given data on this socket
    /// </summary>
    public static async Task SendAsJson(this ClientWebSocket socket, object value)
    {
        var jsonMessage = JsonConvert.SerializeObject(value);
        var buffer = Encoding.UTF8.GetBytes(jsonMessage);
        await socket.SendAsync(new ArraySegment<byte>(buffer), WebSocketMessageType.Text, true, CancellationToken.None);
    }
    
    /// <summary>
    /// Receives data on this socket, and notifies the given callback
    /// </summary>
    public static async Task HandleMessages(this ClientWebSocket socket, Action<string> onReceive, CancellationToken ct)
    {
        var buffer = new byte[1024];

        while (!ct.IsCancellationRequested && socket.State == WebSocketState.Open)
        {
            var result = await socket.ReceiveAsync(new ArraySegment<byte>(buffer), CancellationToken.None);

            if (result.MessageType != WebSocketMessageType.Text)
                continue;

            var message = Encoding.UTF8.GetString(buffer, 0, result.Count);

            try
            {
                onReceive?.Invoke(message);
            }
            catch (Exception e)
            {
                Log.Error($"Error while handling message ('{e.Message}'): {message}");
            }
        }
    }
}