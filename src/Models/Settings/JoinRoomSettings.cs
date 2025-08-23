using BingoAPI.Entities.Clients;

namespace BingoAPI.Models.Settings;

/// <summary>
/// Data used to join a room when calling <see cref="BaseClient.JoinRoom"/>
/// </summary>
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
    /// Name of the player to connect as
    /// </summary>
    public string Nickname { get; set; }
    
    /// <summary>
    /// Should the user be connected as a spectator or not
    /// </summary>
    public bool IsSpectator { get; set; }
}