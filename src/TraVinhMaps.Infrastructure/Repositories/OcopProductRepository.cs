// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Threading;
using MongoDB.Bson;
using MongoDB.Driver;
using TraVinhMaps.Application.Features.OcopProduct.Models;
using TraVinhMaps.Application.UnitOfWorks;
using TraVinhMaps.Domain.Entities;
using TraVinhMaps.Infrastructure.Db;

namespace TraVinhMaps.Infrastructure.CustomRepositories;
public class OcopProductRepository : BaseRepository<OcopProduct>, IOcopProductRepository
{
    private readonly IMongoCollection<Interaction> _interactionCollection;
    private readonly IMongoCollection<User> _userCollection;
    private readonly IMongoCollection<InteractionLogs> _interactionLogsCollection;

    public OcopProductRepository(IDbContext dbContext) : base(dbContext)
    {
        _interactionCollection = dbContext.Database.GetCollection<Interaction>("Interaction");
        _userCollection = dbContext.Database.GetCollection<User>("User");
        _interactionLogsCollection = dbContext.Database.GetCollection<InteractionLogs>("InteractionLogs");

        // Tạo index để tối ưu hiệu suất
        _interactionCollection.Indexes.CreateOne(new CreateIndexModel<Interaction>(
            Builders<Interaction>.IndexKeys.Ascending(i => i.ItemId).Ascending(i => i.ItemType).Ascending(i => i.CreatedAt)));
        _userCollection.Indexes.CreateOne(new CreateIndexModel<User>(
            Builders<User>.IndexKeys.Ascending("favorites.favorite_itemId").Ascending("favorites.favorite_type")));
        _interactionLogsCollection.Indexes.CreateOne(new CreateIndexModel<InteractionLogs>(
            Builders<InteractionLogs>.IndexKeys.Ascending(i => i.ItemId).Ascending(i => i.ItemType).Ascending(i => i.CreatedAt)));
    }
    public async Task<OcopProduct> GetOcopProductByName(string name, CancellationToken cancellationToken = default)
    {
        var filter = Builders<OcopProduct>.Filter.Eq(c => c.ProductName, name) & Builders<OcopProduct>.Filter.Eq(s => s.Status, true);
        return await _collection.Find(filter).FirstOrDefaultAsync(cancellationToken);
    }
    public async Task<IEnumerable<OcopProduct>> GetOcopProductByCompanyId(string companyId, CancellationToken cancellationToken = default)
    {
        var filter = Builders<OcopProduct>.Filter.Eq(c => c.CompanyId, companyId) & Builders<OcopProduct>.Filter.Eq(s => s.Status, true);
        var ocopProduct = await _collection.Find(filter).ToListAsync();
        return ocopProduct;
    }

    public async Task<IEnumerable<OcopProduct>> GetOcopProductByOcopTypeId(string ocopTypeId, CancellationToken cancellationToken = default)
    {
        var filter = Builders<OcopProduct>.Filter.Eq(c => c.OcopTypeId, ocopTypeId) & Builders<OcopProduct>.Filter.Eq(s => s.Status, true);
        var ocopProduct = await _collection.Find(filter).ToListAsync();
        return ocopProduct;
    }
    public async Task<bool> DeleteOcopProductAsync(string id, CancellationToken cancellationToken = default)
    {
        var filter = Builders<OcopProduct>.Filter.Eq(o => o.Id, id);
        var restore = Builders<OcopProduct>.Update.Set(r => r.Status, false);
        var restoreOcopProduct = await _collection.UpdateOneAsync(filter, restore);
        return restoreOcopProduct.IsAcknowledged && restoreOcopProduct.ModifiedCount < 0;
    }
    public async Task<bool> RestoreOcopProductAsync(string id, CancellationToken cancellationToken = default)
    {
        var filter = Builders<OcopProduct>.Filter.Eq(o => o.Id, id);
        var restore = Builders<OcopProduct>.Update.Set(r => r.Status, true);
        var restoreOcopProduct = await _collection.UpdateOneAsync(filter, restore);
        return restoreOcopProduct.IsAcknowledged && restoreOcopProduct.ModifiedCount < 0;
    }

    public async Task<string> AddImageOcopProduct(string id, string imageUrl, CancellationToken cancellationToken = default)
    {
        var filter = Builders<OcopProduct>.Filter.Eq(o => o.Id, id);
        var ocopProduct = await _collection.Find(filter).FirstOrDefaultAsync(cancellationToken);
        if (ocopProduct == null) return null;
        if (ocopProduct.ProductImage == null)
        {
            var newListImage = Builders<OcopProduct>.Update.Set(im => im.ProductImage, new List<string>());
            await _collection.UpdateOneAsync(filter, newListImage);
        }
        var updateImage = Builders<OcopProduct>.Update.Push(p => p.ProductImage, imageUrl);
        var updateResult = await _collection.UpdateOneAsync(filter, updateImage, cancellationToken: cancellationToken);
        return imageUrl;
    }
    public async Task<SellLocation> AddSellLocation(string id, SellLocation sellLocation, CancellationToken cancellationToken = default)
    {
        var filter = Builders<OcopProduct>.Filter.Eq(o => o.Id, id);
        var ocopProduct = await _collection.Find(filter).FirstOrDefaultAsync(cancellationToken);
        if (ocopProduct == null) return null;
        if (ocopProduct.Sellocations == null)
        {
            var newListSellLocation = Builders<OcopProduct>.Update.Set(im => im.Sellocations, new List<SellLocation>());
            await _collection.UpdateOneAsync(filter, newListSellLocation);
        }
        var updateSellLocaton = Builders<OcopProduct>.Update.Push(p => p.Sellocations, sellLocation);
        var updateResult = await _collection.UpdateOneAsync(filter, updateSellLocaton, cancellationToken: cancellationToken);
        return sellLocation;
    }

    public async Task<bool> DeleteSellLocation(string ocopProductId, string sellLocationName, CancellationToken cancellationToken = default)
    {
        var filter = Builders<OcopProduct>.Filter.And(Builders<OcopProduct>.Filter.Eq(o => o.Id, ocopProductId), Builders<OcopProduct>.Filter.ElemMatch(p => p.Sellocations, s => s.LocationName == sellLocationName));
        var update = Builders<OcopProduct>.Update.PullFilter(p => p.Sellocations, s => s.LocationName == sellLocationName);
        var result = await _collection.UpdateOneAsync(filter, update, cancellationToken: cancellationToken);
        return result.IsAcknowledged && result.ModifiedCount > 0;
    }

    public async Task<bool> UpdateSellLocation(string ocopProductId, SellLocation sellLocation, CancellationToken cancellationToken = default)
    {
        var filter = Builders<OcopProduct>.Filter.Eq(o => o.Id, ocopProductId);
        var ocopProduct = await _collection.Find(filter).FirstOrDefaultAsync(cancellationToken);
        if (ocopProduct == null || ocopProduct.Sellocations == null) return false;
        var index = ocopProduct.Sellocations.FindIndex(s => s.LocationName == sellLocation.LocationName);
        ocopProduct.Sellocations[index] = sellLocation;
        var update = Builders<OcopProduct>.Update.Set(o => o.Sellocations, ocopProduct.Sellocations);
        var result = await _collection.UpdateOneAsync(filter, update, cancellationToken: cancellationToken);
        return result.IsAcknowledged && result.ModifiedCount > 0;
    }

    // ViewCount: Số lượt xem(từ bảng InteractionLogs)

    // InteractionCount: Số lượng tương tác(từ bảng Interaction)

    // WishlistCount: Số lượt sản phẩm được thêm vào yêu thích(từ bảng User.favorites)
    public async Task<IEnumerable<OcopProductAnalytics>> GetProductAnalyticsAsync(
    string timeRange = "month",
    DateTime? startDate = null,
    DateTime? endDate = null,
    CancellationToken cancellationToken = default)
    {
        // Get the current UTC time
        var now = DateTime.UtcNow;

        DateTime filterStartDate;
        DateTime? filterEndDate = null;

        // Determine the filter range
        if (startDate.HasValue && endDate.HasValue)
        {
            // Use explicit date range if provided
            filterStartDate = startDate.Value;
            filterEndDate = endDate.Value;
        }
        else
        {
            // Use relative time range based on the selected timeRange
            filterStartDate = timeRange.ToLower() switch
            {
                "day" => now.AddDays(-1),
                "week" => now.AddDays(-7),
                "month" => new DateTime(now.Year, now.Month, 1),
                "year" => new DateTime(now.Year, 1, 1),
                _ => new DateTime(now.Year, now.Month, 1)
            };
        }

        // Convert C# DateTime to BsonValue for MongoDB usage
        var bsonStartDate = BsonValue.Create(filterStartDate);
        var bsonEndDate = filterEndDate.HasValue ? BsonValue.Create(filterEndDate.Value) : null;

        // Filter conditions for Interaction collection
        var interactionCond = new BsonArray
    {
        new BsonDocument("$eq", new BsonArray { "$$interaction.itemType", "Ocop Product" }),
        new BsonDocument("$gte", new BsonArray { "$$interaction.createdAt", bsonStartDate })
    };
        if (bsonEndDate != null)
        {
            interactionCond.Add(new BsonDocument("$lte", new BsonArray { "$$interaction.createdAt", bsonEndDate }));
        }

        // Filter conditions for InteractionLogs collection
        var logCond = new BsonArray
    {
        new BsonDocument("$eq", new BsonArray { "$$log.itemType", "Ocop Product" }),
        new BsonDocument("$gte", new BsonArray { "$$log.createdAt", bsonStartDate })
    };
        if (bsonEndDate != null)
        {
            logCond.Add(new BsonDocument("$lte", new BsonArray { "$$log.createdAt", bsonEndDate }));
        }

        // MongoDB aggregation pipeline to gather analytics
        var pipeline = new List<BsonDocument>
    {
        // Step 1: Match only active products
        new("$match", new BsonDocument("status", true)),

        // Step 2: Project essential fields and convert _id to string
        new("$project", new BsonDocument
        {
            { "_id", 1 },
            { "productName", 1 },
            { "stringId", new BsonDocument("$toString", "$_id") }
        }),

        // Step 3: Lookup interactions for each product
        new("$lookup", new BsonDocument
        {
            { "from", "Interaction" },
            { "localField", "stringId" },
            { "foreignField", "itemId" },
            { "as", "interactions" }
        }),

        // Step 4: Filter interactions by itemType and date
        new("$addFields", new BsonDocument("interactions",
            new BsonDocument("$filter", new BsonDocument
            {
                { "input", "$interactions" },
                { "as", "interaction" },
                { "cond", new BsonDocument("$and", interactionCond) }
            })
        )),

        // Step 5: Lookup view logs (InteractionLogs) for each product
        new("$lookup", new BsonDocument
        {
            { "from", "InteractionLogs" },
            { "localField", "stringId" },
            { "foreignField", "itemId" },
            { "as", "interactionLogs" }
        }),

        // Step 6: Filter view logs by itemType and date
        new("$addFields", new BsonDocument("interactionLogs",
            new BsonDocument("$filter", new BsonDocument
            {
                { "input", "$interactionLogs" },
                { "as", "log" },
                { "cond", new BsonDocument("$and", logCond) }
            })
        )),

        // Step 7: Lookup users who have favorited this product
        new("$lookup", new BsonDocument
        {
            { "from", "User" },
            { "localField", "stringId" },
            { "foreignField", "favorites.itemId" },
            { "as", "users" }
        }),

        // Step 8: Final projection with computed counts
        new("$project", new BsonDocument
        {
            { "Id", "$stringId" },
            { "ProductName", "$productName" },
            { "ViewCount", new BsonDocument("$size", "$interactionLogs") },
            { "InteractionCount", new BsonDocument("$sum", "$interactions.totalCount") },
            { "WishlistCount", new BsonDocument("$size",
                new BsonDocument("$reduce",
                    new BsonDocument
                    {
                        { "input", "$users" },
                        { "initialValue", new BsonArray() },
                        { "in", new BsonDocument("$concatArrays",
                            new BsonArray
                            {
                                "$$value",
                                new BsonDocument("$filter",
                                    new BsonDocument
                                    {
                                        { "input", "$$this.favorites" },
                                        { "as", "fav" },
                                        { "cond", new BsonDocument("$eq",
                                            new BsonArray { "$$fav.itemId", "$stringId" })
                                        }
                                    }
                                )
                            }
                        )}
                    }
                )
            )}
        })
    };

        // Execute pipeline and retrieve results
        var rawAnalytics = await _collection.Aggregate<BsonDocument>(pipeline, null, cancellationToken).ToListAsync();

        // Map raw Bson results to strongly-typed DTOs
        return rawAnalytics.Select(b => new OcopProductAnalytics
        {
            Id = b.GetValue("Id", "").AsString,
            ProductName = b.GetValue("ProductName", "").AsString,
            ViewCount = b.GetValue("ViewCount", 0).ToInt64(),
            InteractionCount = b.GetValue("InteractionCount", 0).ToInt64(),
            WishlistCount = b.GetValue("WishlistCount", 0).ToInt64()
        });
    }

}
