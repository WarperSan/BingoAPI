#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member

namespace BingoAPI.Models;

/// <summary>
/// Teams available for a bingo match
/// </summary>
[Flags]
public enum Team : ushort
{
	// No team
	Blank = 0,

	// All team colors
	Pink = 1 << 1,
	Red = 1 << 2,
	Orange = 1 << 3,
	Brown = 1 << 4,
	Yellow = 1 << 5,
	Green = 1 << 6,
	Teal = 1 << 7,
	Blue = 1 << 8,
	Navy = 1 << 9,
	Purple = 1 << 10
}
