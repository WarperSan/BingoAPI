using BingoAPI.Events;
using Newtonsoft.Json;

namespace BingoAPI.Networking.DTOs;

/// <summary>
/// Model used as the response's payload of <see cref="BingoApiClient.GetFeed"/>
/// </summary>
internal record GetFeedResponse
{
	[JsonProperty("events")]
	[JsonRequired]
	public readonly IBingoEvent[] Events = [];
}
