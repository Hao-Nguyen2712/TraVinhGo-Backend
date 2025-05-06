// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using MongoDB.Bson.Serialization.Attributes;

namespace TraVinhMaps.Domain.Entities;

/// <summary>
/// abstract class for entities
/// </summary>
public abstract class BaseEntity
{
    /// <summary>
    /// Gets or sets the identifier.
    /// </summary>
    /// <value>
    /// The identifier.
    /// </value>
    [BsonId]
    [BsonRepresentation(MongoDB.Bson.BsonType.ObjectId)]
    public required string Id { get; set; }

    /// <summary>
    /// Gets or sets the created at.
    /// </summary>
    /// <value>
    /// The created at.
    /// </value>
    [BsonElement("createdAt")]
    [BsonDateTimeOptions(Kind = DateTimeKind.Utc)]
    public required DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}
