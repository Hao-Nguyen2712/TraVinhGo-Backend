using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MongoDB.Bson.Serialization.Attributes;

namespace TraVinhMaps.Domain.Entities
{
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
        [BsonRepresentation(MongoDB.Bson.BsonType.String)]
        public required Guid SessionId { get; set; }
        /// <summary>
        /// Gets or sets the refresh token.
        /// </summary>
        /// <value>
        /// The refresh token.
        /// </value>
        [BsonElement("refreshToken")]
        [BsonRepresentation(MongoDB.Bson.BsonType.String)]
        public required Guid RefreshToken { get; set; }

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
}
