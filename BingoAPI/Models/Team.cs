using System.Runtime.Serialization;
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
#pragma warning disable CS1591
	[EnumMember(Value = "blank")]
	None = 0,

	[EnumMember(Value = "pink")]
	Pink = 1 << 0,

	[EnumMember(Value = "red")]
	Red = 1 << 1,

	[EnumMember(Value = "orange")]
	Orange = 1 << 2,

	[EnumMember(Value = "brown")]
	Brown = 1 << 3,

	[EnumMember(Value = "yellow")]
	Yellow = 1 << 4,

	[EnumMember(Value = "green")]
	Green = 1 << 5,

	[EnumMember(Value = "teal")]
	Teal = 1 << 6,

	[EnumMember(Value = "blue")]
	Blue = 1 << 7,

	[EnumMember(Value = "navy")]
	Navy = 1 << 8,

	[EnumMember(Value = "purple")]
	Purple = 1 << 9,
#pragma warning restore CS1591
}
