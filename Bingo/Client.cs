using System;
using System.Net.WebSockets;
using System.Threading;
using System.Threading.Tasks;
using BingoAPI.Extensions;
using BingoAPI.Helpers;
using BingoAPI.Models.Events;
using BingoAPI.Network;
using Newtonsoft.Json.Linq;

namespace BingoAPI.Bingo;

public abstract class Client
{
    private ClientWebSocket? _socket;
    
    ~Client() => Disconnect();

    internal virtual async Task<bool> Connect(ClientWebSocket socket) => await socket.HandleTimeout();

    public virtual async void Disconnect()
    {
        if (_socket == null)
        {
            Logger.Debug("Tried to dispose a client that has no socket.");
            return;
        }

        try
        {
            await _socket.CloseAsync(WebSocketCloseStatus.NormalClosure, "Client disconnecting", CancellationToken.None);
        }
        catch (Exception e)
        {
            Logger.Error(e.Message);
        }

        _socket.Dispose();
        _socket = null;
    }

    internal void AssignSocket(ClientWebSocket socket)
    {
        _socket = socket;
        _ = socket.HandleMessages(OnSocketReceived);
    }

    private void OnSocketReceived(JObject? json)
    {
        if (json == null)
            return;

        var @event = Event.ParseEvent(json);
        
        if (@event == null)
            return;

        HandleEvent(@event);
    }

    /// <summary>
    /// Called when this client needs to handle a new event
    /// </summary>
    protected abstract void HandleEvent(Event @event);
}