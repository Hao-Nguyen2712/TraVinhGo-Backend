// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using MongoDB.Bson.Serialization.Attributes;

namespace TraVinhMaps.Domain.Entities;

/// <summary>
/// represents a one-time password (OTP) entity in the system.
/// </summary>
/// <seealso cref="TraVinhMaps.Domain.Entities.BaseEntity" />
public class Otp : BaseEntity
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
    /// Gets or sets the otp code.
    /// </summary>
    /// <value>
    /// The otp code.
    /// </value>
    [BsonElement("otpCode")]
    public required string OtpCode { get; set; }

    /// <summary>
    /// Gets or sets the expired at.
    /// </summary>
    /// <value>
    /// The expired at.
    /// </value>
    [BsonElement("expiredAt")]
    [BsonDateTimeOptions(Kind = DateTimeKind.Utc)]
    public required DateTime ExpiredAt { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether this instance is used.
    /// </summary>
    /// <value>
    ///   <c>true</c> if this instance is used; otherwise, <c>false</c>.
    /// </value>
    [BsonElement("isUsed")]
    public required bool IsUsed { get; set; }
}
