// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using MongoDB.Bson.Serialization.Attributes;

namespace TraVinhMaps.Domain.Entities;
/// <summary>
/// Contact document objectl
/// </summary>
public class Contact
{
    [BsonElement("phone")]
    public string? Phone { get; set; }

    [BsonElement("email")]
    public string? Email { get; set; }
    [BsonElement("website")]
    public string? Website { get; set; }
}
