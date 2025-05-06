// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using MongoDB.Bson.Serialization.Attributes;

namespace TraVinhMaps.Domain.Entities;

/// <summary>
/// represents a role entity in the system.
/// </summary>
/// <seealso cref="TraVinhMaps.Domain.Entities.BaseEntity" />
public class Role : BaseEntity
{
    /// <summary>
    /// Gets or sets the name of the role.
    /// </summary>
    /// <value>
    /// The name of the role.
    /// </value>
    [BsonElement("roleName")]
    public required string RoleName { get; set; }
    /// <summary>
    /// Gets or sets a value indicating whether [role status].
    /// </summary>
    /// <value>
    ///   <c>true</c> if [role status]; otherwise, <c>false</c>.
    /// </value>
    [BsonElement("roleStatus")]
    [BsonRepresentation(MongoDB.Bson.BsonType.Boolean)]
    public required bool RoleStatus { get; set; }
}
