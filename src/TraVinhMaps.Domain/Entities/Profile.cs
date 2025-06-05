// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using MongoDB.Bson.Serialization.Attributes;

namespace TraVinhMaps.Domain.Entities;

/// <summary>
/// profile document object
/// </summary>
public class Profile
{
    [BsonElement("fullName")]
    public string? FullName { get; set; }

    [BsonElement("dateOfBirth")]
    public DateOnly? DateOfBirth { get; set; }

    [BsonElement("phoneNumber")]
    public string? PhoneNumber { get; set; }

    [BsonElement("gender")]
    public string? Gender { get; set; }

    [BsonElement("address")]
    public string? Address { get; set; }
    [BsonElement("avatar")]
    public string? Avatar { get; set; }
}
