// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using MongoDB.Bson.Serialization.Attributes;

namespace TraVinhMaps.Domain.Entities;
/// <summary>
/// Represents a review entity in the system.
/// </summary>
/// <seealso cref="TraVinhMaps.Domain.Entities.BaseEntity" />
public class Review : BaseEntity
{
    /// <summary>Gets or sets the rating point.</summary>
    /// <value>The rating point.</value>
    [BsonElement("rating")]
    [BsonRepresentation(MongoDB.Bson.BsonType.Int32)]
    public int Rating { get; set; }

    /// <summary>Gets or sets the images.</summary>
    /// <value>The images.</value>
    [BsonElement("images")]
    public List<string>? Images { get; set; }

    /// <summary>Gets or sets the comment.</summary>
    /// <value>The comment.</value>
    [BsonElement("comment")]
    public string? Comment { get; set; }
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
    /// Gets or sets the reply.
    /// </summary>
    /// <value>
    /// The reply.
    /// </value>
    [BsonElement("reply")]
    public List<Reply>? Reply { get; set; }
    /// <summary>
    /// Gets or sets the destination identifier.
    /// </summary>
    /// <value>
    /// The destination identifier.
    /// </value>
    [BsonElement("destinationId")]
    [BsonRepresentation(MongoDB.Bson.BsonType.ObjectId)]
    public required string DestinationId { get; set; }
}
