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
    [BsonElement("actualIdentifier")]
    public required string Identifier { get; set; }

    [BsonElement("identifierType")]
    public required string IdentifierType { get; set; }
    /// <summary>
    /// Gets or sets the otp code.
    /// </summary>
    /// <value>
    /// The otp code.
    /// </value>

    [BsonElement("hashedOtpCode")]
    public required string HashedOtpCode { get; set; }

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

    /// <summary>
    /// Gets or sets the number of failed verification attempts for this OTP.
    /// Can be used for rate limiting or locking out an OTP after too many failed attempts.
    /// </summary>
    [BsonElement("attemptCount")]
    public int? AttemptCount { get; set; }

    /// <summary>
    /// Gets or sets the date and time when this OTP was last attempted for verification.
    /// Useful for more sophisticated attempt tracking.
    /// </summary>
    [BsonElement("lastAttemptAt")]
    [BsonDateTimeOptions(Kind = DateTimeKind.Utc)]
    public DateTime? LastAttemptAt { get; set; }
}
