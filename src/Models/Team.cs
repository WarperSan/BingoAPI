using System;
#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member

namespace BingoAPI.Models;

/// <summary>
/// Teams available for a bingo match
/// </summary>
[Flags]
public enum Team: ushort
{
    // No team
    BLANK = 0,
        
    // All team colors
    PINK = 1 << 1,
    RED = 1 << 2,
    ORANGE = 1 << 3,
    BROWN = 1 << 4,
    YELLOW = 1 << 5,
    GREEN = 1 << 6,
    TEAL = 1 << 7,
    BLUE = 1 << 8,
    NAVY = 1 << 9,
    PURPLE = 1 << 10
}