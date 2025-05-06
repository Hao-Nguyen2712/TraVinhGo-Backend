// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using MongoDB.Bson.Serialization.Attributes;

namespace TraVinhMaps.Domain.Entities;
/// <summary>
/// Represents a local specialty entity in the system.
/// </summary>
/// <seealso cref="TraVinhMaps.Domain.Entities.BaseEntity" />
public class LocalSpeacialties : BaseEntity
{
    /// <summary>
    /// Gets or sets the name of the food.
    /// </summary>
    /// <value>
    /// The name of the food.
    /// </value>
    [BsonElement("foodName")]
    public required string FoodName { get; set; }

    /// <summary>
    /// Gets or sets the description of the specialty.
    /// </summary>
    /// <value>
    /// The description.
    /// </value>
    [BsonElement("description")]
    public string? Description { get; set; }

    /// <summary>
    /// Gets or sets the images of the specialty.
    /// </summary>
    /// <value>
    /// The images.
    /// </value>
    [BsonElement("images")]
    [BsonRepresentation(MongoDB.Bson.BsonType.Array)]
    public List<string>? Images { get; set; }

    /// <summary>
    /// Gets or sets the locations where this specialty can be found.
    /// </summary>
    /// <value>
    /// The locations.
    /// </value>
    [BsonElement("locations")]
    [BsonRepresentation(MongoDB.Bson.BsonType.Array)]
    public required List<LocalSpecialtyLocation> Locations { get; set; }

    /// <summary>
    /// Gets or sets the tag identifier.
    /// </summary>
    /// <value>
    /// The tag identifier.
    /// </value>
    [BsonElement("tagId")]
    [BsonRepresentation(MongoDB.Bson.BsonType.ObjectId)]
    public required string TagId { get; set; }

    /// <summary>
    /// Gets or sets the status of the specialty.
    /// </summary>
    /// <value>
    ///   <c>true</c> if status; otherwise, <c>false</c>.
    /// </value>
    [BsonElement("status")]
    [BsonRepresentation(MongoDB.Bson.BsonType.Boolean)]
    public required bool Status { get; set; }
}
