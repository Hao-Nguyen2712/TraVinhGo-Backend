using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MongoDB.Bson.Serialization.Attributes;

namespace TraVinhMaps.Domain.Entities;

/// <summary>
/// represents a search history entity in the system.
/// </summary>
/// <seealso cref="TraVinhMaps.Domain.Entities.BaseEntity" />
public class SearchHistory : BaseEntity
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
    /// Gets or sets the query.
    /// </summary>
    /// <value>
    /// The query.
    /// </value>
    [BsonElement("query")]
    public string? Query { get; set; }

    /// <summary>
    /// Gets or sets the type.
    /// </summary>
    /// <value>
    /// The type.
    /// </value>
    [BsonElement("type")]
    public string? Type { get; set; }

    /// <summary>
    /// Returns true if <see langword="search"/> is valid.
    /// </summary>
    /// <value>
    ///   <c>true</c> if this instance is valid; otherwise, <c>false</c>.
    /// </value>
    [BsonElement("isValid")]
    public required bool IsValid { get; set; }
}
