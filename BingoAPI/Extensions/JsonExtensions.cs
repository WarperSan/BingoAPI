using System.Diagnostics.CodeAnalysis;
using JetBrains.Annotations;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace BingoAPI.Extensions;

/// <summary>
/// Class holding extension methods for <see cref="Newtonsoft"/>
/// </summary>
[PublicAPI]
[SuppressMessage("ReSharper", "ConvertToExtensionBlock")]
public static class JsonExtensions
{
	/// <summary>
	/// Gets the required property with the given name
	/// </summary>
	public static JToken GetRequired(this JObject obj, string propertyName)
	{
		if (!obj.TryGetValue(propertyName, out var token))
			throw new JsonException($"Missing required property '{propertyName}'.");

		return token;
	}

	/// <summary>
	/// Gets the required propert with the given name of the type <typeparamref name="T"/>
	/// </summary>
	public static T GetRequired<T>(this JObject obj, string propertyName)
	{
		var token = obj.GetRequired(propertyName);

		var value = token.Value<T>();

		if (value == null)
			throw new JsonException(
				$"Property '{propertyName}' could not be read as a '{typeof(T)}'."
			);

		return value;
	}
}
