using BingoAPI.Extensions;
using BingoAPI.Models;
using Newtonsoft.Json.Linq;

namespace BingoAPI.Events;

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
}
