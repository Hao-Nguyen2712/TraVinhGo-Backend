// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using MongoDB.Bson.Serialization.Attributes;

namespace TraVinhMaps.Domain.Entities;
/// <summary>
/// Represents a tips entity in the system.
/// </summary>
/// <seealso cref="TraVinhMaps.Domain.Entities.BaseEntity" />
public class Tips : BaseEntity
{
    /// <summary>
    /// Gets or sets the title of the tip.
    /// </summary>
    /// <value>
    /// The title.
    /// </value>
    [BsonElement("title")]
    public required string Title { get; set; }

    /// <summary>
    /// Gets or sets the content of the tip.
    /// </summary>
    /// <value>
    /// The content.
    /// </value>
    [BsonElement("content")]
    public required string Content { get; set; }

    /// <summary>
    /// Gets or sets the update date.
    /// </summary>
    /// <value>
    /// The update date.
    /// </value>
    [BsonElement("updateAt")]
    [BsonDateTimeOptions(Kind = DateTimeKind.Utc)]
    public DateTime? UpdateAt { get; set; }

    /// <summary>
    /// Gets or sets the status of the tip.
    /// </summary>
    /// <value>
    ///   <c>true</c> if status is active; otherwise, <c>false</c>.
    /// </value>
    [BsonElement("status")]
    [BsonRepresentation(MongoDB.Bson.BsonType.Boolean)]
    public required bool Status { get; set; }

    /// <summary>
    /// Gets or sets the tag identifier.
    /// </summary>
    /// <value>
    /// The tag identifier.
    /// </value>
    [BsonElement("tagId")]
    [BsonRepresentation(MongoDB.Bson.BsonType.ObjectId)]
    public required string TagId { get; set; }
}
