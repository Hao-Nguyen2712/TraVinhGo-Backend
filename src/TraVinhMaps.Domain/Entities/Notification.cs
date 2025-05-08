// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using MongoDB.Bson.Serialization.Attributes;

namespace TraVinhMaps.Domain.Entities;
/// <summary>
/// represents a notification entity in the system.
/// </summary>
/// <seealso cref="TraVinhMaps.Domain.Entities.BaseEntity" />
public class Notification : BaseEntity
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
    /// Gets or sets the title.
    /// </summary>
    /// <value>
    /// The title.
    /// </value>
    [BsonElement("title")]
    public string? Title { get; set; }
    /// <summary>
    /// Gets or sets the content.
    /// </summary>
    /// <value>
    /// The content.
    /// </value>
    [BsonElement("content")]
    public string? Content { get; set; }
    /// <summary>
    /// Gets or sets a value indicating whether this instance is read.
    /// </summary>
    /// <value>
    ///   <c>true</c> if this instance is read; otherwise, <c>false</c>.
    /// </value>
    [BsonElement("isRead")]
    public required bool IsRead { get; set; }
}

