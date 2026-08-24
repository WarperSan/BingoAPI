using BingoAPI.Clients;
using BingoAPI.Clients.BingoSync;
using BingoAPI.Converters.BingoSync;
using BingoAPI.Events;
using BingoAPI.Events.BingoSync;
using BingoAPI.Logging;
using BingoAPI.Network;
using JetBrains.Annotations;
using Newtonsoft.Json;

namespace BingoAPI.Servers.BingoSync;

/// <summary>
/// Default implementation of <see cref="IBingoServer"/> for BingoSync
/// </summary>
[PublicAPI]
public sealed class BingoSyncServer : IBingoServer, IDisposable
{
	private readonly HttpClient _httpClient;

	/// <summary>
	/// Initializes a new instance of the <see cref="BingoSyncServer"/> class.
	/// </summary>
	public BingoSyncServer(Uri baseUri, Uri webSocketUri, ILogger logger)
	{
		var serializerSettings = new JsonSerializerSettings
		{
			Converters = [new BingoSyncTeamConverter(), new BingoSyncSquareConverter()],
		};

		var requestBuilder = new RequestBuilder().ToUri(baseUri);

		_httpClient = new HttpClient();

		var apiClient = new BingoSyncApiClient(_httpClient, requestBuilder, serializerSettings);

		var socketClient = new BingoSyncSocketClient(webSocketUri, logger);

		EventHandler = new BingoSyncEventHandler(logger);
		var eventProvider = new BingoSyncEventProvider(serializerSettings);

		SessionClient = new BingoSyncSessionClient(
			apiClient,
			socketClient,
			EventHandler,
			eventProvider,
			logger
		);
	}

	/// <summary>
	/// Initializes a new instance of the <see cref="BingoSyncServer"/> class.
	/// </summary>
	public BingoSyncServer(ILogger logger)
		: this(
			new Uri("https://bingosync.com/"),
			new Uri("wss://sockets.bingosync.com/broadcast"),
			logger
		) { }

	/// <inheritdoc />
	public IEventHandler EventHandler { get; }

	/// <inheritdoc />
	public IBingoSessionClient SessionClient { get; }

	/// <inheritdoc />
	public void Dispose()
	{
		_httpClient.Dispose();
	}
}
