// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using MongoDB.Bson.Serialization.Attributes;

namespace TraVinhMaps.Domain.Entities;
public class Company : BaseEntity
{
    [BsonElement("name")]
    public required string Name { get; set; }
    [BsonElement("address")]
    public required string Address { get; set; }
    [BsonElement("location")]
    public required List<Location> Locations { get; set; }
    [BsonElement("contact")]
    public Contact? Contact { get; set; }

}
