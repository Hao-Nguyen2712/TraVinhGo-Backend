// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using MongoDB.Bson.Serialization.Attributes;

namespace TraVinhMaps.Domain.Entities;
/// <summary>
/// Represents a product entity in the system.
/// </summary>
/// <seealso cref="TraVinhMaps.Domain.Entities.BaseEntity" />
public class OcopProduct : BaseEntity
{
    /// <summary>
    /// Gets or sets the name of the product.
    /// </summary>
    /// <value>
    /// The name of the product.
    /// </value>
    [BsonElement("productName")]
    public required string ProductName { get; set; }

    /// <summary>
    /// Gets or sets the product description.
    /// </summary>
    /// <value>
    /// The product description.
    /// </value>
    [BsonElement("productDescription")]
    public string? ProductDescription { get; set; }

    /// <summary>
    /// Gets or sets the product image.
    /// </summary>
    /// <value>
    /// The product image.
    /// </value>
    [BsonElement("productImage")]
    public List<string>? ProductImage { get; set; }

    /// <summary>
    /// Gets or sets the product price.
    /// </summary>
    /// <value>
    /// The product price.
    /// </value>
    [BsonElement("productPrice")]
    public string? ProductPrice { get; set; }
    /// <summary>
    /// Gets or sets the ocop type identifier.
    /// </summary>
    /// <value>
    /// The ocop type identifier.
    /// </value>
    [BsonElement("ocopTypeId")]
    [BsonRepresentation(MongoDB.Bson.BsonType.ObjectId)]
    public required string OcopTypeId { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether this <see cref="OcopProduct"/> is status.
    /// </summary>
    /// <value>
    ///   <c>true</c> if status; otherwise, <c>false</c>.
    /// </value>
    [BsonElement("status")]
    [BsonRepresentation(MongoDB.Bson.BsonType.Boolean)]
    public required bool Status { get; set; }

    /// <summary>
    /// Gets or sets the update at.
    /// </summary>
    /// <value>
    /// The update at.
    /// </value>
    [BsonElement("updateAt")]
    [BsonDateTimeOptions(Kind = DateTimeKind.Utc)]
    public DateTime? UpdateAt { get; set; } = DateTime.UtcNow;
    /// <summary>
    /// Gets or sets the sellocations.
    /// </summary>
    /// <value>
    /// The sellocations.
    /// </value>
    [BsonElement("sellLocations")]
    public List<SellLocation>? Sellocations { get; set; }
    /// <summary>
    /// Gets or sets the company identifier.
    /// </summary>
    /// <value>
    /// The company identifier.
    /// </value>
    [BsonElement("companyId")]
    [BsonRepresentation(MongoDB.Bson.BsonType.ObjectId)]
    public required string CompanyId { get; set; }
    /// <summary>
    /// Gets or sets the ocop point.
    /// </summary>
    /// <value>
    /// The ocop point.
    /// </value>
    [BsonElement("ocopPoint")]
    [BsonRepresentation(MongoDB.Bson.BsonType.Int32)]
    public required int OcopPoint { get; set; }
    /// <summary>
    /// Gets or sets the ocop year release.
    /// </summary>
    /// <value>
    /// The ocop year release.
    /// </value>
    [BsonElement("ocopYearRelease")]
    [BsonRepresentation(MongoDB.Bson.BsonType.Int32)]
    public required int OcopYearRelease { get; set; }
    /// <summary>
    /// Gets or sets the tag identifier.
    /// </summary>
    /// <value>
    /// The tag identifier.
    /// </value>
    [BsonElement("tagId")]
    [BsonRepresentation(MongoDB.Bson.BsonType.ObjectId)]
    public required string TagId { get; set; }
    /// <summary>
    /// Gets or sets the link identifier.
    /// </summary>
    /// <value>
    /// The link identifier.
    /// </value>
    [BsonElement("sellingLinkId")]
    [BsonRepresentation(MongoDB.Bson.BsonType.ObjectId)]
    public required string SellingLinkId { get; set; }
}
