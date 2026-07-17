using System.Net.WebSockets;
using System.Text;
using BingoAPI.Helpers;
using Newtonsoft.Json;

namespace BingoAPI.Networking.Clients;

/// <summary>
/// Handles all communication with the BingoSync websocket
/// </summary>
internal sealed class BingoSocketClient : IDisposable
{
	private WebSocket? _socket;
	private CancellationTokenSource? _cts;
	private Task? _socketReceiveTask;

	private readonly Uri _broadcastUri;

	public BingoSocketClient(Uri socketAddress)
	{
		var builder = new UriBuilder(socketAddress)
		{
			Path = "broadcast",
		};

		_broadcastUri = builder.Uri;
	}

	/// <summary>
	/// Opens a <see cref="WebSocket"/> using the given key
	/// </summary>
	public async Task Connect(
		string socketKey,
		Action<string> onMessageReceived,
		CancellationToken ct
	)
	{
		if (_socket != null)
			throw new InvalidOperationException("Socket is already connected.");

		var socket = new ClientWebSocket();

		try
		{
			await socket.ConnectAsync(_broadcastUri, ct);

			var json = JsonConvert.SerializeObject(new
			{
				socket_key = socketKey,
			});

			await socket.SendAsync(
				new ArraySegment<byte>(
					Encoding.UTF8.GetBytes(json)
				),
				WebSocketMessageType.Text,
				true,
				ct
			);

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
	public async Task Disconnect(CancellationToken ct)
	{
		if (_socket == null)
			return;

		_cts?.Cancel();

		if (_socket.State == WebSocketState.Open)
		{
			try
			{
				await _socket.CloseAsync(
					WebSocketCloseStatus.NormalClosure,
					"Client disconnecting",
					ct
				);
			}
			catch (Exception ex)
			{
				Log.Error($"Error closing WebSocket: {ex.Message}");
			}
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
			catch (Exception ex)
			{
				Log.Error($"Receive loop failed during disconnect: {ex.Message}");
			}
		}

		CleanUp();
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

		using var ms = new MemoryStream();

		while (!ct.IsCancellationRequested && socket.State == WebSocketState.Open)
		{
			WebSocketReceiveResult result;

			do
			{
				result = await socket.ReceiveAsync(
					new ArraySegment<byte>(buffer),
					ct
				);
				ms.Write(buffer, 0, result.Count);
			} while (!result.EndOfMessage);

			if (result.MessageType == WebSocketMessageType.Close)
			{
				Log.Debug("Close message was received.");
				break;
			}

			if (result.MessageType == WebSocketMessageType.Text)
			{
				var message = Encoding.UTF8.GetString(ms.ToArray());

				Log.Debug("Message received:\n" + message);

				try
				{
					onReceive.Invoke(message);
				}
				catch (Exception ex)
				{
					Log.Error($"Error handling socket message: {ex}");
				}
			}

			ms.Seek(0, SeekOrigin.Begin);
			ms.SetLength(0);
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
