using Newtonsoft.Json;

namespace BingoAPI.Extensions;

internal static class HttpResponseMessageExtensions
{
	/// <summary>
	/// Parses the JSON payload from the given <see cref="HttpResponseMessage"/>
	/// </summary>
	public static async Task<T> ParseJson<T>(this HttpResponseMessage response)
	{
		var responseBody = await response.Content.ReadAsStringAsync();
		var typedResponse = JsonConvert.DeserializeObject<T>(responseBody);

		// ReSharper disable once ConvertIfStatementToReturnStatement
		if (typedResponse == null)
			throw new InvalidOperationException($"Failed to deserialize response to {typeof(T).Name}");

		return typedResponse;
	}
}
