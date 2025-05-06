// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using MongoDB.Bson.Serialization.Attributes;

namespace TraVinhMaps.Domain.Entities;
/// <summary>
/// Represents an itineraryplan entity in the system.
/// </summary>
/// <seealso cref="TraVinhMaps.Domain.Entities.BaseEntity" />
public class ItineraryPlan : BaseEntity
{
    /// <summary>
    /// Gets or sets the name of the itinerary plan.
    /// </summary>
    /// <value>
    /// The name of the itinerary plan.
    /// </value>
    [BsonElement("name")]
    public required string Name { get; set; }

    /// <summary>
    /// Gets or sets the duration of the itinerary plan.
    /// </summary>
    /// <value>
    /// The duration.
    /// </value>
    [BsonElement("duration")]
    public string? Duration { get; set; }

    /// <summary>
    /// Gets or sets the locations in the itinerary plan.
    /// </summary>
    /// <value>
    /// The locations.
    /// </value>
    [BsonElement("locations")]
    [BsonRepresentation(MongoDB.Bson.BsonType.Array)]
    public List<string>? Locations { get; set; }

    /// <summary>
    /// Gets or sets the start date of the itinerary plan.
    /// </summary>
    /// <value>
    /// The start date.
    /// </value>
    [BsonElement("startDate")]
    [BsonDateTimeOptions(Kind = DateTimeKind.Utc)]
    public required DateTime StartDate { get; set; }

    /// <summary>
    /// Gets or sets the end date of the itinerary plan.
    /// </summary>
    /// <value>
    /// The end date.
    /// </value>
    [BsonElement("endDate")]
    [BsonDateTimeOptions(Kind = DateTimeKind.Utc)]
    public required DateTime EndDate { get; set; }

    /// <summary>
    /// Gets or sets the estimated cost of the itinerary plan.
    /// </summary>
    /// <value>
    /// The estimated cost.
    /// </value>
    [BsonElement("estimatedCost")]
    public string? EstimatedCost { get; set; }

    /// <summary>
    /// Gets or sets the status of the itinerary plan.
    /// </summary>
    /// <value>
    ///   <c>true</c> if status is active; otherwise, <c>false</c>.
    /// </value>
    [BsonElement("status")]
    [BsonRepresentation(MongoDB.Bson.BsonType.Boolean)]
    public required bool Status { get; set; }

    /// <summary>
    /// Gets or sets the update date.
    /// </summary>
    /// <value>
    /// The update date.
    /// </value>
    [BsonElement("updateAt")]
    [BsonDateTimeOptions(Kind = DateTimeKind.Utc)]
    public DateTime? UpdateAt { get; set; }
}
