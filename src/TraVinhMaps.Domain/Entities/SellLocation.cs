// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using MongoDB.Bson.Serialization.Attributes;

namespace TraVinhMaps.Domain.Entities;
/// <summary>
/// sell location document object
/// </summary>
public class SellLocation
{
    [BsonElement("locationName")]
    public string? LocationName { get; set; }
    [BsonElement("locationAddress")]
    public string? LocationAddress { get; set; }
    [BsonElement("markerId")]
    [BsonRepresentation(MongoDB.Bson.BsonType.ObjectId)]
    public required string MarkerId { get; set; }
    [BsonElement("location")]
    public Location? Location { get; set; }
}
