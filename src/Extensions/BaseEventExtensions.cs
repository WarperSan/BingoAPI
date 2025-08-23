using BingoAPI.Entities.Clients;
using BingoAPI.Entities.Events;

namespace BingoAPI.Extensions;

/// <summary>
/// Provides extension methods for <see cref="BaseEvent"/>
/// </summary>
internal static class BaseEventExtensions
{
    /// <summary>
    /// Checks if this event was caused by the given client
    /// </summary>
    public static bool IsFromLocal(this BaseEvent @event, BaseClient client) => client.UUID == @event.Player.UUID;
}