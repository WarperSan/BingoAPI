namespace BingoAPI.Models.Settings;

public struct CreateRoomSettings
{
    /// <summary>
    /// Name of the room
    /// </summary>
    public string Name { get; set; }
    
    /// <summary>
    /// Password of the room
    /// </summary>
    public string Password { get; set; }
    
    /// <summary>
    /// Nickname of the user to log as
    /// </summary>
    public string Nickname { get; set; }
    
    /// <summary>
    /// Should the room be randomized or not
    /// </summary>
    public bool IsRandomized { get; set; }

    /// <summary>
    /// Should the room be in lockout or not
    /// </summary>
    public bool IsLockout { get; set; }

    /// <summary>
    /// Seed to use for the randomization
    /// </summary>
    /// <remarks>
    /// Leave it empty if you want the seed to be automatically generated
    /// </remarks>
    public string Seed { get; set; }

    /// <summary>
    /// Goals to generate the board from
    /// </summary>
    public Goal[] Goals { get; set; }
    
    /// <summary>
    /// Should the user be connected as a spectator or not
    /// </summary>
    public bool IsSpectator { get; set; }
    
    /// <summary>
    /// Should the card be hidden at the start of the game or not
    /// </summary>
    public bool HideCard { get; set; }

    internal int VariantType => IsRandomized ? 172 : 18; // 18 = Fixed Board, 172 = Randomized
    internal int LockoutMode => IsLockout ? 2 : 1; // 1 = Non-Lockout, 2 = Lockout
}