using System.Net.WebSockets;
using BingoAPI.Extensions;
using BingoAPI.Helpers;

namespace BingoAPI.Network;

internal sealed class SocketHandler : IDisposable
{
	private ClientWebSocket? _socket;
	private CancellationTokenSource? _cts;
	private Task? _socketReceiveTask;

	/// <summary>
	/// Opens the <see cref="WebSocket"/> using the given key
	/// </summary>
	public async Task<bool> Connect(string socketKey, Action<string> onMessageReceived)
	{
		var socket = await Request.CreateSocket(
			"wss://sockets.bingosync.com/broadcast",
			socketKey
		);

		if (socket == null)
			return false;

		_cts = new CancellationTokenSource();

		_socket = socket;
		_socketReceiveTask = _socket.HandleMessages(onMessageReceived, _cts.Token);

		return true;
	}

	/// <summary>
	/// Closes the <see cref="WebSocket"/> gracefully
	/// </summary>
	public async Task Disconnect()
	{
		_cts?.Cancel();

		try
		{
			if (_socket != null && _socket.State == WebSocketState.Open)
			{
				await _socket.CloseAsync(
					WebSocketCloseStatus.NormalClosure,
					"Client disconnecting",
					CancellationToken.None
				);
			}
		}
		catch (Exception e)
		{
			Log.Error($"Error closing WebSocket: {e.Message}");
		}

		if (_socketReceiveTask != null)
		{
			try
			{
				await _socketReceiveTask;
			}
			catch (OperationCanceledException)
			{ /* Expected */
			}
			catch (ObjectDisposedException)
			{ /* Expected */
			}
			catch (Exception ex)
			{
				Log.Error($"Error in receive task during disconnect: {ex.Message}");
			}
		}

		CleanUp();
	}

	private void CleanUp()
	{
		_socket?.Dispose();
		_socket = null;

		_cts?.Dispose();
		_cts = null;

		_socketReceiveTask = null;
	}

	/// <inheritdoc />
	public void Dispose()
	{
		_socket?.Dispose();
		_cts?.Dispose();
		_socketReceiveTask?.Dispose();
	}
}
