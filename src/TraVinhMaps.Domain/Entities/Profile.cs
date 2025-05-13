using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MongoDB.Bson.Serialization.Attributes;

namespace TraVinhMaps.Domain.Entities;

/// <summary>
/// profile document object
/// </summary>
public class Profile
{
    [BsonElement("fullName")]
    public string? FullName { get; set; }

    [BsonElement("phoneNumber")]
    public string? PhoneNumber { get; set; }
    [BsonElement("address")]
    public string? Address { get; set; }
    [BsonElement("avatar")]
    public string? Avatar { get; set; }
}
