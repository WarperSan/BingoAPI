using BingoAPI.Models;
using UnityEngine.Events;

namespace BingoAPI.Managers;

/// <summary>
/// Class that offers static calling when events are received
/// </summary>
public static class ClientEventManager
{
    /// <summary>
    /// Called when the local client gets connected
    /// </summary>
    public static readonly UnityEvent<string?, PlayerData> OnSelfConnected = new();

    /// <summary>
    /// Called when the local client gets disconnected
    /// </summary>
    public static readonly UnityEvent OnSelfDisconnected = new();

    /// <summary>
    /// Called when the local client marks a goal
    /// </summary>
    public static readonly UnityEvent<PlayerData, SquareData> OnSelfMarked = new();

    /// <summary>
    /// Called when the local client clears a goal
    /// </summary>
    public static readonly UnityEvent<PlayerData, SquareData> OnSelfCleared = new();

    /// <summary>
    /// Called when the local client sends a message
    /// </summary>
    public static readonly UnityEvent<PlayerData, string, ulong> OnSelfChatted = new();

    /// <summary>
    /// Called when the local client changes team
    /// </summary>
    public static readonly UnityEvent<PlayerData, Team, Team> OnSelfTeamChanged = new();
    
    /// <summary>
    /// Called when another client gets connected
    /// </summary>
    public static readonly UnityEvent<string?, PlayerData> OnOtherConnected = new();

    /// <summary>
    /// Called when another client gets disconnected
    /// </summary>
    public static readonly UnityEvent<string?, PlayerData> OnOtherDisconnected = new();
    
    /// <summary>
    /// Called when another client marks a goal
    /// </summary>
    public static readonly UnityEvent<PlayerData, SquareData> OnOtherMarked = new();
    
    /// <summary>
    /// Called when another client clears a goal
    /// </summary>
    public static readonly UnityEvent<PlayerData, SquareData> OnOtherCleared = new();
    
    /// <summary>
    /// Called when another client sends a message
    /// </summary>
    public static readonly UnityEvent<PlayerData, string, ulong> OnOtherChatted = new();
    
    /// <summary>
    /// Called when another client changes team
    /// </summary>
    public static readonly UnityEvent<PlayerData, Team, Team> OnOtherTeamChanged = new();
}