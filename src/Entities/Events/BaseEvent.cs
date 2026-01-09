using BingoAPI.Extensions;
using BingoAPI.Helpers;
using BingoAPI.Models;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace BingoAPI.Entities.Events;

/// <summary>
/// Class that represents the events sent from the server
/// </summary>
public abstract class BaseEvent
{
    /// <summary>
    /// Player responsible for this event
    /// </summary>
    public readonly PlayerData Player;

    /// <summary>
    /// Team responsible for this event
    /// </summary>
    public readonly Team Team;

    /// <summary>
    /// Time when this event was sent
    /// </summary>
    public readonly ulong Timestamp;

    internal BaseEvent(JObject json)
    {
        Player = new PlayerData(json.GetValue("player"));
        Team = json.Value<string>("player_color").GetTeam();
        Timestamp = json.Value<ulong>("timestamp");
    }

    #region Parsing

    /// <summary>
    /// Parses the given JSON to the appropriate event
    /// </summary>
    public static BaseEvent? ParseEvent(string content)
    {
        var json = JsonConvert.DeserializeObject<JObject>(content);

        if (json == null)
        {
            Log.Error($"Could not create a JSON object with the given event: {content}");
            return null;
        }

        return ParseEvent(json);
    }

    /// <summary>
    /// Parses the given JSON to the appropriate event
    /// </summary>
    public static BaseEvent? ParseEvent(JObject json)
    {
        var type = json.Value<string>("type") ?? "";

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

        Log.Error($"Unhandled event: {json}");
        return null;
    }
    #endregion
}