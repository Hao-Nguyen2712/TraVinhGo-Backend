// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using MongoDB.Bson.Serialization.Attributes;

namespace TraVinhMaps.Domain.Entities;
/// <summary>
/// Event location document object
/// </summary>
public class EventLocation
{
    [BsonElement("name")]
    public string? Name { get; set; }

    [BsonElement("address")]
    public string? Address { get; set; }

    [BsonElement("location")]
    [BsonRepresentation(MongoDB.Bson.BsonType.Document)]
    public Location? location { get; set; }

    [BsonElement("markerId")]
    [BsonRepresentation(MongoDB.Bson.BsonType.ObjectId)]
    public required string MarkerId { get; set; }
}
