// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using MongoDB.Bson.Serialization.Attributes;

namespace TraVinhMaps.Domain.Entities;

public class InteractionLogs : BaseEntity
{
    /// <summary>
    /// Gets or sets the user identifier.
    /// </summary>
    /// <value>
    /// The user identifier.
    /// </value>
    [BsonElement("userId")]
    [BsonRepresentation(MongoDB.Bson.BsonType.ObjectId)]
    public required string UserId { get; set; }

    /// <summary>
    /// Gets or sets the item identifier.
    /// </summary>
    /// <value>
    /// The item identifier.
    /// </value>
    [BsonElement("itemId")]
    [BsonRepresentation(MongoDB.Bson.BsonType.ObjectId)]
    public required string ItemId { get; set; }

    /// <summary>
    /// Gets or sets the type of the item (e.g., product, destination, event, local food).
    /// </summary>
    /// <value>
    /// The type of the item.
    /// </value>
    [BsonElement("itemType")]
    public string? ItemType { get; set; }

    /// <summary>
    /// Gets or sets the duration of the interaction in seconds.
    /// </summary>
    /// <value>
    /// The duration in seconds.
    /// </value>
    [BsonElement("duration")]
    [BsonRepresentation(MongoDB.Bson.BsonType.Int32)]
    public int? Duration { get; set; }
}
