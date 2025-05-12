// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using MongoDB.Bson.Serialization.Attributes;

namespace TraVinhMaps.Domain.Entities;

public class UserSession : BaseEntity
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
    /// Gets or sets the session identifier.
    /// </summary>
    /// <value>
    /// The session identifier.
    /// </value>
    [BsonElement("sessionId")]
    public required string SessionId { get; set; }
    /// <summary>
    /// Gets or sets the refresh token.
    /// </summary>
    /// <value>
    /// The refresh token.
    /// </value>
    [BsonElement("refreshToken")]
    public required string RefreshToken { get; set; }

    /// <summary>
    /// Gets or sets the refresh token expire at.
    /// </summary>
    /// <value>
    /// The refresh token expire at.
    /// </value>
    [BsonElement("refreshTokenExpireAt")]
    [BsonDateTimeOptions(Kind = DateTimeKind.Utc)]
    public required DateTime RefreshTokenExpireAt { get; set; }

    /// <summary>
    /// Gets or sets the device information.
    /// </summary>
    /// <value>
    /// The device information.
    /// </value>
    [BsonElement("deviceInfo")]
    public string? DeviceInfo { get; set; }

    /// <summary>
    /// Gets or sets the ip address.
    /// </summary>
    /// <value>
    /// The ip address.
    /// </value>
    [BsonElement("ipAddress")]
    public string? IpAddress { get; set; }
    //override the update
    /// <summary>
    /// Gets or sets the expire at.
    /// </summary>
    /// <value>
    /// The expire at.
    /// </value>
    [BsonElement("expireAt")]
    [BsonDateTimeOptions(Kind = DateTimeKind.Utc)]
    public DateTime? ExpireAt { get; set; }
    /// <summary>
    /// Gets or sets a value indicating whether this instance is active.
    /// </summary>
    /// <value>
    ///   <c>true</c> if this instance is active; otherwise, <c>false</c>.
    /// </value>
    [BsonElement("isActive")]
    public required bool IsActive { get; set; }
}
