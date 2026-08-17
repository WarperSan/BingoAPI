using System.Diagnostics.CodeAnalysis;
using System.Net;
using JetBrains.Annotations;

namespace BingoAPI.Extensions;

/// <summary>
/// Class holding extension methods for <see cref="HttpResponseMessage"/>
/// </summary>
[PublicAPI]
[SuppressMessage("ReSharper", "ConvertToExtensionBlock")]
public static class HttpResponseMessageExtensions
{
	/// <summary>
	/// Gets the value of the cookie with the given name or a default value if not found
	/// </summary>
	public static string? GetCookieOrDefault(this HttpResponseMessage response, string cookieName)
	{
		var setCookies = response.Headers.GetValues("Set-Cookie");

		var container = new CookieContainer();
		var containerUri = response.RequestMessage.RequestUri;

		foreach (var setCookie in setCookies)
			container.SetCookies(containerUri, setCookie);

		var cookies = container.GetCookies(containerUri);

		return cookies[cookieName]?.Value;
	}
}
