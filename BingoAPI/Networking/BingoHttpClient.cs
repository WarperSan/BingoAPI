using System.Net.Http.Headers;
using System.Net.Mime;
using System.Text;
using Newtonsoft.Json;
using UnityEngine;

namespace BingoAPI.Networking;

/// <summary>
/// Wrapper around <see cref="HttpClient"/>
/// </summary>
internal sealed class BingoHttpClient : IDisposable
{
	private readonly HttpClient _client;

	public BingoHttpClient()
	{
		_client = new HttpClient();
		_client.BaseAddress = new Uri("https://bingosync.com");
		_client.Timeout = TimeSpan.FromSeconds(30);

		_client.DefaultRequestHeaders.UserAgent.Add(new ProductInfoHeaderValue(
			Plugin.Id,
			Plugin.Version
		));
	}

	private async Task<HttpResponseMessage> SendJsonAsync(
		HttpMethod method,
		string endpoint,
		object payload,
		CancellationToken ct
	)
	{
		var json = JsonConvert.SerializeObject(payload);

		using var request = new HttpRequestMessage();
		request.Method = method;
		request.RequestUri = new Uri(endpoint, UriKind.Relative);

		request.Content = new StringContent(
			json,
			Encoding.UTF8,
			MediaTypeNames.Application.Json
		);

		return await _client.SendAsync(request, ct);
	}

	/// <summary>
	/// Sends a request with the given JSON
	/// </summary>
	/// <exception cref="InvalidOperationException"></exception>
	public async Task<T> SendJson<T>(
		HttpMethod method,
		string endpoint,
		object payload,
		CancellationToken ct = default
	)
	{
		using var response = await SendJsonAsync(
			method,
			endpoint,
			payload,
			ct
		);

		response.EnsureSuccessStatusCode();

		var responseBody = await response.Content.ReadAsStringAsync();
		var typedResponse = JsonConvert.DeserializeObject<T>(responseBody);

		// ReSharper disable once ConvertIfStatementToReturnStatement
		if (typedResponse == null)
			throw new InvalidOperationException($"Failed to deserialize response to {typeof(T).Name}");

		return typedResponse;
	}

	/// <summary>
	/// Sends a request with the given JSON
	/// </summary>
	public async Task SendJson(
		HttpMethod method,
		string endpoint,
		object payload,
		CancellationToken ct = default
	)
	{
		using var response = await SendJsonAsync(
			method,
			endpoint,
			payload,
			ct
		);

		response.EnsureSuccessStatusCode();
	}

	/// <inheritdoc />
	public void Dispose()
	{
		_client.Dispose();
	}
}
