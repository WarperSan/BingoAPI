using BingoAPI.Clients.BingoSync;
using Newtonsoft.Json;

namespace BingoAPI.DTOs.BingoSync.Connect;

/// <summary>
/// Model used as the payload of <see cref="BingoSyncSocketClient.Connect(string,Action{string},CancellationToken)"/>
/// </summary>
internal record Request
{
	[JsonProperty("socket_key")]
	[JsonRequired]
	public required string SocketKey { get; init; }
}
