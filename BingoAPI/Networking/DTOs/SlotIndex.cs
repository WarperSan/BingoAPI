using BingoAPI.Networking.Converters;
using Newtonsoft.Json;

namespace BingoAPI.Networking.DTOs;

/// <summary>
/// Wrapper around <see cref="int"/> to identify a bingo slot
/// </summary>
[JsonConverter(typeof(SlotIndexConverter))]
internal record SlotIndex
{
	internal SlotIndex(int index)
	{
		Index = index;
	}

	/// <summary>
	/// Index of this slot
	/// </summary>
	public readonly int Index;
}
