// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using MongoDB.Bson.Serialization.Attributes;

namespace TraVinhMaps.Domain.Entities;
/// <summary>
/// Represents an event and festival entity in the system.
/// </summary>
/// <seealso cref="TraVinhMaps.Domain.Entities.BaseEntity" />
public class EventAndFestival : BaseEntity
{
    /// <summary>
    /// Gets or sets the name of the event.
    /// </summary>
    /// <value>
    /// The name of the event.
    /// </value>
    [BsonElement("nameEvent")]
    public required string NameEvent { get; set; }

    /// <summary>
    /// Gets or sets the description of the event.
    /// </summary>
    /// <value>
    /// The description.
    /// </value>
    [BsonElement("description")]
    public string? Description { get; set; }

    /// <summary>
    /// Gets or sets the start date of the event.
    /// </summary>
    /// <value>
    /// The start date.
    /// </value>
    [BsonElement("startDate")]
    [BsonDateTimeOptions(Kind = DateTimeKind.Utc)]
    public required DateTime StartDate { get; set; }

    /// <summary>
    /// Gets or sets the end date of the event.
    /// </summary>
    /// <value>
    /// The end date.
    /// </value>
    [BsonElement("endDate")]
    [BsonDateTimeOptions(Kind = DateTimeKind.Utc)]
    public required DateTime EndDate { get; set; }

    /// <summary>
    /// Gets or sets the category of the event (Su kien hay le Hoi).
    /// </summary>
    /// <value>
    /// The category.
    /// </value>
    [BsonElement("category")]
    public string? Category { get; set; }

    /// <summary>
    /// Gets or sets the images of the event.
    /// </summary>
    /// <value>
    /// The images.
    /// </value>
    [BsonElement("images")]
    [BsonRepresentation(MongoDB.Bson.BsonType.Array)]
    public required List<string> Images { get; set; }

    /// <summary>
    /// Gets or sets the event location information.
    /// </summary>
    /// <value>
    /// The location.
    /// </value>
    [BsonElement("location")]
    [BsonRepresentation(MongoDB.Bson.BsonType.Document)]
    public required EventLocation Location { get; set; }

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
    /// Gets or sets the status of the event.
    /// </summary>
    /// <value>
    ///   <c>true</c> if status; otherwise, <c>false</c>.
    /// </value>
    [BsonElement("status")]
    [BsonRepresentation(MongoDB.Bson.BsonType.Boolean)]
    public required bool Status { get; set; }
}
