using BingoAPI.Networking.Converters;
using Newtonsoft.Json;

namespace BingoAPI.Models;

/// <summary>
/// Teams available for a bingo match
/// </summary>
[Flags]
[JsonConverter(typeof(TeamConverter))]
public enum Team : ushort
{
	None = 0,
	Pink = 1 << 0,
	Red = 1 << 1,
	Orange = 1 << 2,
	Brown = 1 << 3,
	Yellow = 1 << 4,
	Green = 1 << 5,
	Teal = 1 << 6,
	Blue = 1 << 7,
	Navy = 1 << 8,
	Purple = 1 << 9,
}
