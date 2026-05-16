using BingoAPI.Events;
using Newtonsoft.Json;

namespace BingoAPI.Networking.DTOs;

/// <summary>
/// Model used as the response's payload of <see cref="BingoApiClient.GetFeed"/>
/// </summary>
public record ApiGetFeedResponse
{
	[JsonProperty("events")]
	[JsonRequired]
	public IBingoEvent[] Events = [];
}
