using System.Reflection;
using System.Runtime.Serialization;
using System.Text;
using JetBrains.Annotations;
using Newtonsoft.Json;

namespace BingoAPI.Network;

/// <summary>
/// Class allowing to build <see cref="HttpRequestMessage"/> with ease
/// </summary>
[PublicAPI]
public sealed class RequestBuilder
{
	/// <summary>
	/// Initializes a new instance of the <see cref="RequestBuilder"/> class.
	/// </summary>
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
	public RequestBuilder WithMethod(HttpMethod method)
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

	private UriBuilder _uriBuilder;

	/// <summary>
	/// Sets the endpoint of this request
	/// </summary>
	public RequestBuilder ToEndpoint(string endpoint)
	{
		_uriBuilder.Path = endpoint;
		return this;
	}

	/// <summary>
	/// Sets the URL of this request
	/// </summary>
	public RequestBuilder ToUri(Uri uri)
	{
		_uriBuilder = new UriBuilder(uri);
		return this;
	}

	#endregion

	#region Content

	private HttpContent? _content;

	/// <summary>
	/// Sets the payload of this request to the given JSON payload
	/// </summary>
	/// <remarks>
	/// This method parses the payload using <see cref="JsonConvert.SerializeObject(object)"/>
	/// </remarks>
	public RequestBuilder WithJson(object json, JsonSerializerSettings? serializerSettings = null)
	{
		var serializedJson = JsonConvert.SerializeObject(json, serializerSettings);

		return WithJson(serializedJson);
	}

	/// <summary>
	/// Sets the payload of this request to the given JSON payload
	/// </summary>
	public RequestBuilder WithJson(string json)
	{
		_content = new StringContent(json, Encoding.UTF8, "application/json");

		return this;
	}

	/// <summary>
	/// Sets the payload of this request to the given form payload
	/// </summary>
	public RequestBuilder WithForm(object form)
	{
		var fields = new List<KeyValuePair<string, string>>();

		var members = form.GetType()
			.GetMembers(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance)
			.Where(m => m.MemberType is MemberTypes.Field or MemberTypes.Property)
			.Select(m => new
			{
				Member = m,
				Attribute = m.GetCustomAttribute<DataMemberAttribute>(),
			})
			.Where(m => m.Attribute != null);

		foreach (var member in members)
		{
			var key = member.Attribute?.Name ?? member.Member.Name;

			var value = member.Member switch
			{
				FieldInfo field => field.GetValue(form)?.ToString(),
				PropertyInfo property => property.GetValue(form)?.ToString(),
				_ => null,
			};

			value ??= "";

			fields.Add(new KeyValuePair<string, string>(key, value));
		}

		_content = new FormUrlEncodedContent(fields);

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
			RequestUri = _uriBuilder.Uri,
			Content = _content,
		};

		return request;
	}
}
