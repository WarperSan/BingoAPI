using System.Diagnostics.CodeAnalysis;
using System.Text.RegularExpressions;

namespace BingoAPI.Helpers;

/// <summary>
/// Class helping for networking stuff
/// </summary>
public static class Network
{
	/// <summary>
	/// Attempts to find the room code from the given URL
	/// </summary>
	public static bool TryGetRoomCode(string url, [NotNullWhen(true)] out string? code)
	{
		var match = Regex.Match(url, "(?<=/room/)[a-zA-Z\\d-_]+");

		if (!match.Success)
		{
			code = null;
			return false;
		}

		code = match.Value;
		return true;
	}
}
