// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using MongoDB.Bson.Serialization.Attributes;

namespace TraVinhMaps.Domain.Entities;
/// <summary>
/// favorite document object
/// </summary>
public class Favorite
{
    [BsonElement("itemId")]
    [BsonRepresentation(MongoDB.Bson.BsonType.ObjectId)]
    public string? ItemId { get; set; }

    [BsonElement("itemType")]
    public string? ItemType { get; set; }

    [BsonElement("updateAt")]
    [BsonDateTimeOptions(Kind = DateTimeKind.Utc)]
    public DateTime? UpdateAt { get; set; } 
}
