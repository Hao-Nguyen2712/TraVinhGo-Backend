// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using MongoDB.Bson.Serialization.Attributes;

namespace TraVinhMaps.Domain.Entities
{
    /// <summary>
    /// Represents a user entity in the system.
    /// </summary>
    public class User : BaseEntity
    {
        /// <summary>
        /// Gets or sets the username of the user.
        /// </summary>
        [BsonElement("username")]
        public string? Username { get; set; }

        /// <summary>
        /// Gets or sets the phone number of the user.
        /// </summary>
        [BsonElement("phoneNumber")]
        public string? PhoneNumber { get; set; }

        /// <summary>
        /// Gets or sets the password of the user.
        /// </summary>
        [BsonElement("password")]
        public required string Password { get; set; }

        /// <summary>
        /// Gets or sets the email of the user.
        /// </summary>
        [BsonElement("email")]
        public string? Email { get; set; }

        /// <summary>
        /// Gets or sets the role identifier.
        /// </summary>
        /// <value>
        /// The role identifier.
        /// </value>
        [BsonElement("roleId")]
        [BsonRepresentation(MongoDB.Bson.BsonType.ObjectId)]
        public required string RoleId { get; set; }

        /// <summary>
        /// Gets or sets the profile of the user.
        /// </summary>
        /// <value>
        /// The profile.
        /// </value>
        [BsonElement("profile")]
        public Profile? Profile { get; set; }

        /// <summary>
        /// Gets or sets the updated at.
        /// </summary>
        /// <value>
        /// The updated at.
        /// </value>
        [BsonElement("updatedAt")]
        [BsonDateTimeOptions(Kind = DateTimeKind.Utc)]
        public DateTime? UpdatedAt { get; set; }

        /// <summary>
        /// Gets or sets the list of favorites associated with the user.
        /// </summary>
        /// <value>
        /// The favorites.
        /// </value>
        /// 
        [BsonElement("favorites")]
        public List<Favorite>? Favorites { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether this instance is forbidden.
        /// </summary>
        /// <value>
        ///   <c>true</c> if this instance is forbidden; otherwise, <c>false</c>.
        /// </value>
        [BsonElement("isForbiden")]
        public bool IsForbidden { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether this <see cref="User"/> is active.
        /// </summary>
        /// <value>
        ///   <c>true</c> if active; otherwise, <c>false</c>.
        /// </value>
        [BsonElement("status")]
        public bool Status { get; set; }
    }
}
