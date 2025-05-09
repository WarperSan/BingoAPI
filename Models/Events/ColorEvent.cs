﻿using Newtonsoft.Json.Linq;

namespace BingoAPI.Models.Events;

/// <summary>
/// Event that represents someone changing team
/// </summary>
public sealed class ColorEvent : Event
{
    public ColorEvent(JObject json) : base(json)
    {
    }
}