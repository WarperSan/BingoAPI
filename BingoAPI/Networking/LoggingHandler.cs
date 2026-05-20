using BingoAPI.Helpers;

namespace BingoAPI.Networking;

/// <summary>
/// Handler used to log requests and responses of <see cref="HttpClient"/>
/// </summary>
internal class LoggingHandler : DelegatingHandler
{
	public LoggingHandler(HttpMessageHandler innerHandler) : base(innerHandler)
	{
	}

	/// <inheritdoc/>
	protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
	{
		var requestPayload = "";

		if (request.Content != null)
			requestPayload = await request.Content.ReadAsStringAsync();

		Log.Debug($"Request:\n{request}\n{requestPayload}");

		var response = await base.SendAsync(request, cancellationToken);

		var responsePayload = "";

		if (response.Content != null)
			responsePayload = await response.Content.ReadAsStringAsync();

		Log.Debug($"Response:\n{response}\n{responsePayload}");

		return response;
	}
}
