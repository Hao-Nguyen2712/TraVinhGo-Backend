// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using MongoDB.Bson.Serialization.Attributes;

namespace TraVinhMaps.Domain.Entities;
/// <summary>
/// opening hours document object
/// </summary>
public class OpeningHours
{
    [BsonElement("openTime")]
    public string? OpenTime { get; set; }
    [BsonElement("closeTime")]
    public string? CloseTime { get; set; }
}
