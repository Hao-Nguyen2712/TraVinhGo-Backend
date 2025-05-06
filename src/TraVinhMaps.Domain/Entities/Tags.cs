// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using MongoDB.Bson.Serialization.Attributes;

namespace TraVinhMaps.Domain.Entities;

/// <summary>
/// represents a tag entity in the system.
/// </summary>
/// <seealso cref="TraVinhMaps.Domain.Entities.BaseEntity" />
public class Tags : BaseEntity
{
    /// <summary>
    /// Gets or sets the name.
    /// </summary>
    /// <value>
    /// The name.
    /// </value>
    [BsonElement("tagName")]
    public required string Name { get; set; }
    /// <summary>
    /// Gets or sets the image.
    /// </summary>
    /// <value>
    /// The image.
    /// </value>
    [BsonElement("tagImage")]
    public string? image { get; set; }
}
