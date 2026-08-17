using System.Diagnostics.CodeAnalysis;
using System.Text.RegularExpressions;
using JetBrains.Annotations;

namespace BingoAPI.Extensions;

/// <summary>
/// Class holding extension methods for <see cref="Uri"/>
/// </summary>
[PublicAPI]
[SuppressMessage("ReSharper", "ConvertToExtensionBlock")]
public static class UriExtensions
{
	/// <summary>
	/// Attempts to find the room code from this <see cref="Uri"/>
	/// </summary>
	public static bool TryGetRoomCode(this Uri uri, [NotNullWhen(true)] out string? code)
	{
		var match = Regex.Match(uri.AbsolutePath, "(?<=/room/)[a-zA-Z\\d-_]+");

		if (!match.Success)
		{
			code = null;
			return false;
		}

		code = match.Value;
		return true;
	}
}
