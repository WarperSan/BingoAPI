using JetBrains.Annotations;
using Newtonsoft.Json;

namespace BingoAPI.Network;

/// <summary>
/// Interface that represents any class that can build <see cref="HttpRequestMessage"/>
/// </summary>
[PublicAPI]
public interface IRequestBuilder
{
	/// <summary>
	/// Sets the HTTP method
	/// </summary>
	IRequestBuilder WithMethod(HttpMethod method);

	/// <summary>
	/// Sets the HTTP method to <see cref="HttpMethod.Get"/>
	/// </summary>
	IRequestBuilder Get();

	/// <summary>
	/// Sets the HTTP method to <see cref="HttpMethod.Post"/>
	/// </summary>
	IRequestBuilder Post();

	/// <summary>
	/// Sets the HTTP method to <see cref="HttpMethod.Put"/>
	/// </summary>
	IRequestBuilder Put();

	/// <summary>
	/// Sets the endpoint of this request
	/// </summary>
	IRequestBuilder ToEndpoint(string endpoint);

	/// <summary>
	/// Sets the payload of this request to the given JSON payload
	/// </summary>
	/// <remarks>
	/// This method parses the payload using <see cref="JsonConvert.SerializeObject(object)"/>
	/// </remarks>
	IRequestBuilder WithJson(object json, JsonSerializerSettings serializerSettings);

	/// <summary>
	/// Sets the payload of this request to the given JSON payload
	/// </summary>
	IRequestBuilder WithJson(string json);

	/// <summary>
	/// Sets the payload of this request to the given form payload
	/// </summary>
	IRequestBuilder WithForm(object form);

	/// <summary>
	/// Builds the <see cref="HttpRequestMessage"/> from this request
	/// </summary>
	HttpRequestMessage Build();
}
