﻿using System;

namespace BingoAPI.Models;

/// <summary>
/// All the teams available for a bingo
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