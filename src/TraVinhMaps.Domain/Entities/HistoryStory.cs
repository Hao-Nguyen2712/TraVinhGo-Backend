// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using MongoDB.Bson.Serialization.Attributes;

namespace TraVinhMaps.Domain.Entities;

/// <summary>
/// History story document object
/// </summary>
public class HistoryStory
{
    [BsonElement("content")]
    public string? Content { get; set; }
    [BsonElement("images")]
    [BsonRepresentation(MongoDB.Bson.BsonType.Array)]
    public List<string>? Images { get; set; }
}
