using System.Text;
using Newtonsoft.Json;

namespace BingoAPI.Networking;

/// <summary>
/// Class allowing to build <see cref="HttpRequestMessage"/> with ease
/// </summary>
internal sealed class RequestBuilder
{
	#region Methods

	private HttpMethod _method = HttpMethod.Get;

	/// <summary>
	/// Sets the HTTP method
	/// </summary>
	private RequestBuilder WithMethod(HttpMethod method)
	{
		_method = method;
		return this;
	}

	/// <summary>
	/// Sets the HTTP method to <see cref="HttpMethod.Get"/>
	/// </summary>
	public RequestBuilder Get() => WithMethod(HttpMethod.Get);

	/// <summary>
	/// Sets the HTTP method to <see cref="HttpMethod.Post"/>
	/// </summary>
	public RequestBuilder Post() => WithMethod(HttpMethod.Post);

	/// <summary>
	/// Sets the HTTP method to <see cref="HttpMethod.Put"/>
	/// </summary>
	public RequestBuilder Put() => WithMethod(HttpMethod.Put);

	#endregion

	#region URI

	private string? _endpoint = null;

	/// <summary>
	/// Sets the endpoint of this request
	/// </summary>
	public RequestBuilder ToEndpoint(string endpoint)
	{
		_endpoint = endpoint;
		return this;
	}

	#endregion

	#region Content

	private HttpContent? _content = null;

	/// <summary>
	/// Sets the JSON payload of this request
	/// </summary>
	public RequestBuilder WithJson(object json)
	{
		var serializedJson = JsonConvert.SerializeObject(json);

		_content = new StringContent(
			serializedJson,
			Encoding.UTF8,
			"application/json"
		);

		return this;
	}

	#endregion

	/// <summary>
	/// Builds the <see cref="HttpRequestMessage"/> from this request
	/// </summary>
	public HttpRequestMessage Build()
	{
		var request = new HttpRequestMessage
		{
			Method = _method,
			Content = _content,
		};

		if (_endpoint != null)
			request.RequestUri = new Uri(_endpoint, UriKind.Relative);

		return request;
	}
}
