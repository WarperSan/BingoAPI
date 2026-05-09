using System.Net.Mime;
using System.Text;
using Newtonsoft.Json;

namespace BingoAPI.Networking;

/// <summary>
/// Class allowing to build <see cref="HttpRequestMessage"/> with ease
/// </summary>
public sealed class RequestBuilder
{
	public RequestBuilder()
	{
		_method = HttpMethod.Get;
		_uriBuilder = new UriBuilder();
		_content = null;
	}

	/// <summary>
	/// Copies this builder to a brand-new builder with the same state
	/// </summary>
	public RequestBuilder(RequestBuilder original)
		: this()
	{
		_uriBuilder = new UriBuilder(original._uriBuilder.Uri);
		_method = original._method;

		if (original._content != null)
		{
			var stream = original._content.ReadAsStreamAsync().GetAwaiter().GetResult();
			_content = new StreamContent(stream);
		}
	}

	#region Methods

	private HttpMethod _method;

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

	private readonly UriBuilder _uriBuilder;

	/// <summary>
	/// Sets the endpoint of this request
	/// </summary>
	public RequestBuilder ToEndpoint(string endpoint)
	{
		_uriBuilder.Path = endpoint;
		return this;
	}

	#endregion

	#region Content

	private HttpContent? _content;

	/// <summary>
	/// Sets the JSON payload of this request
	/// </summary>
	public RequestBuilder WithJson(object json)
	{
		var serializedJson = JsonConvert.SerializeObject(json);

		var jsonContent = new StringContent(
			serializedJson,
			Encoding.UTF8,
			MediaTypeNames.Application.Json
		);

		return WithContent(jsonContent);
	}

	/// <summary>
	/// Sets the payload of this request
	/// </summary>
	public RequestBuilder WithContent(HttpContent content)
	{
		_content = content;
		return this;
	}

	#endregion

	/// <summary>
	/// Builds the <see cref="HttpRequestMessage"/> from this request
	/// </summary>
	public HttpRequestMessage Build()
	{
		var tempBuilder = new UriBuilder(_uriBuilder.Uri);

		var request = new HttpRequestMessage
		{
			Method = _method,
			RequestUri = tempBuilder.Uri,
			Content = _content,
		};

		return request;
	}
}
