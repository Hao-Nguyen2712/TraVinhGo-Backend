// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using MongoDB.Bson.Serialization.Attributes;

namespace TraVinhMaps.Domain.Entities;
/// <summary>
/// Local specialty location document object
/// </summary>
public class LocalSpecialtyLocation
{
    [BsonElement("name")]
    public required string Name { get; set; }

    [BsonElement("address")]
    public required string Address { get; set; }

    [BsonElement("markerId")]
    [BsonRepresentation(MongoDB.Bson.BsonType.ObjectId)]
    public required string MarkerId { get; set; }

    [BsonElement("location")]
    public required Location Location { get; set; }
}
