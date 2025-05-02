using BingoAPI.Extensions;
using BingoAPI.Helpers;
using BingoAPI.Models;
using Newtonsoft.Json.Linq;

namespace BingoAPI.Events;

/// <summary>
/// Class that handles the events sent from the bingo server
/// </summary>
public abstract class Event
{
    public readonly PlayerData Player;
    public readonly Team Team;
    public readonly ulong Timestamp;

    internal Event(JObject json)
    {
        Player = PlayerData.ParseJSON(json.GetValue("player"));
        Team = json.Value<string>("player_color").GetTeam();
        Timestamp = json.Value<ulong>("timestamp");
    }

    public static Event? ParseEvent(JObject json)
    {
        Logger.Debug(json);

        var type = json.Value<string>("type");
        
        switch (type)
        {
            case "connection":
                var eventType = json.Value<string>("event_type");
                
                switch (eventType)
                {
                    case "connected":
                        return new ConnectedEvent(json);
                    case "disconnected":
                        return new DisconnectedEvent(json);
                }
                break;
            case "chat":
                return new ChatEvent(json);
            case "color":
                return new ColorEvent(json);
            case "goal":
                return new GoalEvent(json);
        }
        
        Logger.Error($"Unhandled response: {json}");
        return null;
    }
}