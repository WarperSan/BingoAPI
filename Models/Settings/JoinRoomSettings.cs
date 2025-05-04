namespace BingoAPI.Models.Settings;

public struct JoinRoomSettings
{
    /// <summary>
    /// Code of the room
    /// </summary>
    public string Code { get; set; }
    
    /// <summary>
    /// Password of the room
    /// </summary>
    public string Password { get; set; }
    
    /// <summary>
    /// Nickname of the user to log as
    /// </summary>
    public string Nickname { get; set; }
    
    /// <summary>
    /// Should the user be connected as a spectator or not
    /// </summary>
    public bool IsSpectator { get; set; }
}