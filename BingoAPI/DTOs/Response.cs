using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace BingoAPI.DTOs;

/// <summary>
/// Wrapper to handle <see cref="HttpResponseMessage"/> better
/// </summary>
public sealed class Response<T>
	where T : class
{
	/// <summary>
	/// Determines if the response has been a success
	/// </summary>
	public readonly bool IsSuccess;

	/// <summary>
	/// JSON content of the response if success
	/// </summary>
	public readonly T? Data;

	private Response(T data)
	{
		IsSuccess = true;
		Data = data;
	}

	/// <summary>
	/// Throws an exception if <see cref="IsSuccess"/> is <see langword="false" />
	/// </summary>
	public void EnsureSuccess(out T data)
	{
		if (!IsSuccess)
			throw new InvalidOperationException("The response was not successful.");

		data =
			Data
			?? throw new NullReferenceException(
				"The response was a success, but the data did not load properly."
			);
	}

	/// <summary>
	/// Creates a new instance of <see cref="Response{T}"/> from the given response
	/// </summary>
	public static Response<T> CreateResponse(
		HttpResponseMessage response,
		string content,
		JsonSerializerSettings serializerSettings
	)
	{
		if (response.IsSuccessStatusCode)
			return HandleSuccess(content, serializerSettings);

		var jToken = JToken.Parse(content);

		throw new NotSupportedException($"Received a payload that was not supported:\n{jToken}");
	}

	private static TPayload ParseJson<TPayload>(
		string content,
		JsonSerializerSettings serializerSettings
	)
	{
		TPayload? json;

		try
		{
			json = JsonConvert.DeserializeObject<TPayload>(content, serializerSettings);
		}
		catch (JsonException e)
		{
			throw new InvalidOperationException(
				$"Failed to deserialize the response: \n\n{content}",
				e
			);
		}

		if (json == null)
			throw new NullReferenceException(
				$"Failed to parse the response's payload as '{nameof(T)}'"
			);

		return json;
	}

	private static Response<T> HandleSuccess(string content, JsonSerializerSettings settings)
	{
		var data = ParseJson<T>(content, settings);

		return new Response<T>(data);
	}
}
