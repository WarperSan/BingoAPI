using BingoAPI.Networking.Converters;
using Newtonsoft.Json;

namespace BingoAPI.Events;

/// <summary>
/// Represents any class that can be used as an event
/// </summary>
[JsonConverter(typeof(EventConverter))]
internal interface IEvent;
