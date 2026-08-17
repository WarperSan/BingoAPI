using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace BingoAPI.Network;

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
	/// Creates a new instance of <see cref="Response{T}"/> from the given response
	/// </summary>
	public static Response<T> CreateResponse(
		HttpResponseMessage response,
		string content,
		JsonSerializer serializer
	)
	{
		if (response.IsSuccessStatusCode)
			return HandleSuccess(content, serializer);

		var jToken = JToken.Parse(content);

		throw new NotSupportedException($"Received a payload that was not supported:\n{jToken}");
	}

	private static TPayload ParseJson<TPayload>(string content, JsonSerializer serializer)
	{
		TPayload? json;

		try
		{
			using var reader = new JsonTextReader(new StringReader(content));

			json = serializer.Deserialize<TPayload>(reader);
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

	private static Response<T> HandleSuccess(string content, JsonSerializer serializer)
	{
		var data = ParseJson<T>(content, serializer);

		return new Response<T>(data);
	}
}
