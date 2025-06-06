// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using MongoDB.Bson.Serialization.Attributes;

namespace TraVinhMaps.Domain.Entities;

/// <summary>
/// represents a ocop type entity in the system.
/// </summary>
/// <seealso cref="TraVinhMaps.Domain.Entities.BaseEntity" />
public class OcopType : BaseEntity
{
    /// <summary>
    /// Gets or sets the name of the ocop type.
    /// </summary>
    /// <value>
    /// The name of the ocop type.
    /// </value>
    [BsonElement("ocopTypeName")]
    public required string OcopTypeName { get; set; }
    /// <summary>
    /// Gets or sets a value indicating whether [ocop type status].
    /// </summary>
    /// <value>
    ///   <c>true</c> if [ocop type status]; otherwise, <c>false</c>.
    /// </value>
    [BsonElement("ocopTypeStatus")]
    public required bool OcopTypeStatus { get; set; }
    /// <summary>
    /// Gets or sets the update at.
    /// </summary>
    /// <value>
    /// The update at.
    /// </value>
    [BsonElement("updateAt")]
    [BsonDateTimeOptions(Kind = DateTimeKind.Utc)]
    public DateTime? UpdateAt { get; set; }
}
