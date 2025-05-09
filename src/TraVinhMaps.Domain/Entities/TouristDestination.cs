// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using MongoDB.Bson.Serialization.Attributes;

namespace TraVinhMaps.Domain.Entities;
/// <summary>
/// </summary>
/// <seealso cref="TraVinhMaps.Domain.Entities.BaseEntity" />
public class TouristDestination : BaseEntity
{
    /// <summary>
    /// Gets or sets the name.
    /// </summary>
    /// <value>
    /// The name.
    /// </value>
    [BsonElement("name")]
    public required string Name { get; set; }
    /// <summary>
    /// Gets or sets the avarage rating.
    /// </summary>
    /// <value>
    /// The avarage rating.
    /// </value>
    [BsonElement("avarageRating")]
    public double? AvarageRating { get; set; }
    /// <summary>
    /// Gets or sets the description.
    /// </summary>
    /// <value>
    /// The description.
    /// </value>
    [BsonElement("description")]
    public string? Description { get; set; }
    /// <summary>
    /// Gets or sets the address.
    /// </summary>
    /// <value>
    /// The address.
    /// </value>
    [BsonElement("address")]
    public required string Address { get; set; }
    /// <summary>
    /// Gets or sets the location.
    /// </summary>
    /// <value>
    /// The location.
    /// </value>
    [BsonElement("location")]
    public required Location Location { get; set; }
    /// <summary>
    /// Gets or sets the images.
    /// </summary>
    /// <value>
    /// The images.
    /// </value>
    [BsonElement("images")]
    public List<string>? Images { get; set; }

    /// <summary>
    /// Gets or sets the history story.
    /// </summary>
    /// <value>
    /// The history story.
    /// </value>
    [BsonElement("historyStory")]
    public HistoryStory? HistoryStory { get; set; }

    /// <summary>
    /// Gets or sets the update at.
    /// </summary>
    /// <value>
    /// The update at.
    /// </value>
    [BsonElement("updateAt")]
    [BsonDateTimeOptions(Kind = DateTimeKind.Utc)]
    public DateTime? UpdateAt { get; set; }

    /// <summary>
    /// Gets or sets the destination type identifier.
    /// </summary>
    /// <value>
    /// The destination type identifier.
    /// </value>
    [BsonElement("destinationTypeId")]
    [BsonRepresentation(MongoDB.Bson.BsonType.ObjectId)]
    public required string DestinationTypeId { get; set; }

    /// <summary>
    /// Gets or sets the opening hours.
    /// </summary>
    /// <value>
    /// The opening hours.
    /// </value>
    [BsonElement("openingHours")]
    public OpeningHours? OpeningHours { get; set; }
    /// <summary>
    /// Gets or sets the capacity.
    /// </summary>
    /// <value>
    /// The capacity.
    /// </value>
    [BsonElement("capacity")]
    public string? Capacity { get; set; }
    /// <summary>
    /// Gets or sets the contact.
    /// </summary>
    /// <value>
    /// The contact.
    /// </value>
    [BsonElement("contact")]
    public Contact? Contact { get; set; }

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
    /// Gets or sets the ticket.
    /// </summary>
    /// <value>
    /// The ticket.
    /// </value>
    [BsonElement("ticket")]
    public string? Ticket { get; set; }

    /// <summary>
    /// Gets or sets the ticket count.
    /// </summary>
    /// <value>
    /// The ticket count.
    /// </value>
    [BsonElement("favoriteCount")]
    [BsonRepresentation(MongoDB.Bson.BsonType.Int32)]
    public int? TicketCount { get; set; }

}
