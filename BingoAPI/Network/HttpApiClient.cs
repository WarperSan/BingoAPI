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
	private readonly JsonSerializer _jsonSerializer;

	/// <summary>
	/// Initializes a new instance of the <see cref="HttpApiClient"/> class.
	/// </summary>
	public HttpApiClient(HttpClient client, JsonSerializerSettings settings)
	{
		_client = client;
		_jsonSerializer = JsonSerializer.Create(settings);
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
	public async Task<T> SendRequest<T>(HttpRequestMessage request, CancellationToken ct)
		where T : class
	{
		var response = await SendRequest(request, ct);
		var content = await response.Content.ReadAsStringAsync();

		JsonReader jsonReader = new JsonTextReader(new StringReader(content));

		var value = _jsonSerializer.Deserialize<T>(jsonReader);

		if (value == null)
			throw new NullReferenceException($"Failed to deserialize JSON to '{typeof(T)}'.");

		return value;
	}
}
