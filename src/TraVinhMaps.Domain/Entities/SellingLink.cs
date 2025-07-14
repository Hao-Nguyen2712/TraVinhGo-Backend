// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using MongoDB.Bson.Serialization.Attributes;

namespace TraVinhMaps.Domain.Entities;
/// <summary>
/// Represents a selling link entity in the system.
/// </summary>
/// <seealso cref="TraVinhMaps.Domain.Entities.BaseEntity" />
public class SellingLink : BaseEntity
{
    [BsonRepresentation(MongoDB.Bson.BsonType.ObjectId)]
    [BsonElement("productId")]
    public required string ProductId { get; set; }
    /// <summary>
    /// Gets or sets the tittle.
    /// </summary>
    /// <value>
    /// The tittle.
    /// </value>
    [BsonElement("tittle")]
    public required string Title { get; set; }

    /// <summary>
    /// Gets or sets the link.
    /// </summary>
    /// <value>
    /// The link.
    /// </value>
    [BsonElement("link")]
    public string Link { get; set; }
    /// <summary>
    /// Gets or sets the update at.
    /// </summary>
    /// <value>
    /// The update at.
    /// </value>
    [BsonElement("updateAt")]
    [BsonDateTimeOptions(Kind = DateTimeKind.Utc)]
    public DateTime? UpdateAt { get; set; }
}
