using System.Net.WebSockets;
using System.Text;
using BingoAPI.Helpers;
using Newtonsoft.Json;

namespace BingoAPI.Networking;

/// <summary>
/// Handles all communication with the BingoSync websocket
/// </summary>
internal sealed class BingoSocketClient : IDisposable
{
	private WebSocket? _socket;
	private CancellationTokenSource? _cts;
	private Task? _socketReceiveTask;

	/// <summary>
	/// Opens a <see cref="WebSocket"/> using the given key
	/// </summary>
	public async Task Connect(string socketKey, Action<string> onMessageReceived)
	{
		if (_socket != null)
			throw new InvalidOperationException("Socket is already connected.");

		var socket = new ClientWebSocket();

		try
		{
			await socket.ConnectAsync(
				new Uri("wss://sockets.bingosync.com/broadcast"),
				CancellationToken.None
			);

			await Authenticate(socket, socketKey);

			_socket = socket;
			_cts = new CancellationTokenSource();

			_socketReceiveTask = ReceiveLoop(
				_socket,
				onMessageReceived,
				_cts.Token
			);
		}
		catch
		{
			socket.Dispose();
			throw;
		}
	}

	/// <summary>
	/// Closes the <see cref="WebSocket"/> gracefully
	/// </summary>
	public async Task Disconnect()
	{
		if (_socket == null)
			return;

		_cts?.Cancel();

		try
		{
			if (_socket.State == WebSocketState.Open)
			{
				await _socket.CloseAsync(
					WebSocketCloseStatus.NormalClosure,
					"Client disconnecting",
					CancellationToken.None
				);
			}
		}
		catch (Exception ex)
		{
			Log.Error($"Error closing WebSocket: {ex.Message}");
		}

		if (_socketReceiveTask != null)
		{
			try
			{
				await _socketReceiveTask;
			}
			catch (OperationCanceledException)
			{
				// Expected
			}
			catch (ObjectDisposedException)
			{
				// Expected
			}
			catch (Exception ex)
			{
				Log.Error($"Receive loop failed during disconnect: {ex.Message}");
			}
		}

		CleanUp();
	}

	/// <summary>
	/// Authenticates the given <see cref="WebSocket"/> using the given key
	/// </summary>
	private static async Task Authenticate(
		WebSocket socket,
		string socketKey
	)
	{
		var json = JsonConvert.SerializeObject(new
		{
			socket_key = socketKey
		});

		var buffer = Encoding.UTF8.GetBytes(json);

		await socket.SendAsync(
			buffer,
			WebSocketMessageType.Text,
			true,
			CancellationToken.None
		);
	}

	/// <summary>
	/// Receives data on the given <see cref="WebSocket"/>, and notifies the given callback
	/// </summary>
	private static async Task ReceiveLoop(
		WebSocket socket,
		Action<string> onReceive,
		CancellationToken ct
	)
	{
		var buffer = new byte[1024];

		while (!ct.IsCancellationRequested && socket.State == WebSocketState.Open)
		{
			var result = await socket.ReceiveAsync(
				buffer,
				ct
			);

			if (result.MessageType == WebSocketMessageType.Close)
				break;

			if (result.MessageType != WebSocketMessageType.Text)
				continue;

			var message = Encoding.UTF8.GetString(buffer, 0, result.Count);

			try
			{
				onReceive.Invoke(message);
			}
			catch (Exception ex)
			{
				Log.Error($"Error handling socket message: {ex.Message}\n{message}");
			}
		}
	}

	/// <summary>
	/// Cleans up this instance
	/// </summary>
	private void CleanUp()
	{
		_socket?.Dispose();
		_cts?.Dispose();

		_socket = null;
		_cts = null;
		_socketReceiveTask = null;
	}

	/// <inheritdoc />
	public void Dispose() => CleanUp();
}
