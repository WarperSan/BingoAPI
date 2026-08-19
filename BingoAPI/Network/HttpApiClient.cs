using BingoAPI.DTOs;
using JetBrains.Annotations;
using Newtonsoft.Json;

namespace BingoAPI.Network;

/// <summary>
/// Wrapper around <see cref="HttpClient"/>
/// </summary>
[PublicAPI]
public class HttpApiClient
{
	private readonly HttpClient _client;
	private readonly JsonSerializerSettings _serializerSettings;

	/// <summary>
	/// Initializes a new instance of the <see cref="HttpApiClient"/> class.
	/// </summary>
	public HttpApiClient(HttpClient client, JsonSerializerSettings serializerSettings)
	{
		_client = client;
		_serializerSettings = serializerSettings;
	}

	/// <summary>
	/// Sends the given request
	/// </summary>
	public Task<HttpResponseMessage> SendRequest(HttpRequestMessage request, CancellationToken ct)
	{
		return _client.SendAsync(request, ct);
	}

	/// <summary>
	/// Sends the given request, and parses the returning JSON
	/// </summary>
	public async Task<Response<T>> SendRequest<T>(HttpRequestMessage request, CancellationToken ct)
		where T : class
	{
		var response = await SendRequest(request, ct);
		var content = await response.Content.ReadAsStringAsync();

		return Response<T>.CreateResponse(response, content, _serializerSettings);
	}
}
