using MongoDB.Bson;
using MongoDB.Driver;
using TraVinhMaps.Application.UnitOfWorks;
using TraVinhMaps.Domain.Entities;
using TraVinhMaps.Infrastructure.Db;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

namespace TraVinhMaps.Infrastructure.CustomRepositories;

public class UserRepository : BaseRepository<User>, IUserRepository
{
    private readonly ILogger<UserRepository> _logger;

    public UserRepository(IDbContext context, ILogger<UserRepository> logger) : base(context)
    {
        _logger = logger;
    }

    public async Task<bool> DeleteUser(string id, CancellationToken cancellationToken = default)
    {
        var filter = Builders<User>.Filter.Eq(u => u.Id, id);
        var update = Builders<User>.Update
            .Set(u => u.IsForbidden, true)
            .Set(u => u.Status, false)
            .Set(u => u.UpdatedAt, DateTime.UtcNow);

        var result = await _collection.UpdateOneAsync(filter, update, cancellationToken: cancellationToken);
        return result.ModifiedCount > 0;
    }

    public Task<User> GetUserByEmailAsync(string email, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    public async Task<Dictionary<string, object>> GetUserStatisticsAsync(string groupBy, string timeRange, CancellationToken cancellationToken = default)
    {
        try
        {
            var now = DateTime.UtcNow;
            var result = new Dictionary<string, object>();

            var ageGroups = new[] { 0, 18, 30, 50, 120 };
            var ageLabels = new[] { "0-18", "18-30", "30-50", "50+" };

            DateTime startDate = timeRange.ToLower() switch
            {
                "day" => now.Date,
                "week" => now.Date.AddDays(-(int)now.DayOfWeek),
                "month" => new DateTime(now.Year, now.Month, 1),
                "year" => new DateTime(now.Year, 1, 1),
                _ => DateTime.MinValue
            };

            // ==== 1. AGE ====
            if (groupBy.ToLower() is "all" or "age")
            {
                // First check dateOfBirth field types in collection
                var typeCheckPipeline = new List<BsonDocument>
            {
                new BsonDocument("$group", new BsonDocument
                {
                    { "_id", new BsonDocument("$type", "$profile.dateOfBirth") },
                    { "count", new BsonDocument("$sum", 1) }
                })
            };

                var typeResults = await _collection.Aggregate<BsonDocument>(typeCheckPipeline).ToListAsync(cancellationToken);
                foreach (var typeResult in typeResults)
                {
                    _logger.LogInformation("Found {Count} documents with dateOfBirth type: {Type}",
                        typeResult["count"], typeResult["_id"]);
                }

                var ageStats = new Dictionary<string, int>();

                for (int i = 0; i < ageLabels.Length; i++)
                {
                    var minAge = ageGroups[i];
                    var maxAge = ageGroups[i + 1];

                    var startDob = now.AddYears(-maxAge).Date;
                    var endDob = now.AddYears(-minAge).Date.AddDays(1).AddTicks(-1);

                    _logger.LogInformation("Processing age range {AgeRange}: startDob={StartDob}, endDob={EndDob}",
                        ageLabels[i], startDob, endDob);

                    var pipeline = new List<BsonDocument>
                {
                    // Match documents with dateOfBirth (any type)
                    new BsonDocument("$match", new BsonDocument("profile.dateOfBirth", new BsonDocument("$exists", true))),

                    // Convert to unified date format
                    new BsonDocument("$addFields", new BsonDocument
                    {
                        { "parsedDob", new BsonDocument("$cond", new BsonArray
                            {
                                new BsonDocument("$eq", new BsonArray
                                {
                                    new BsonDocument("$type", "$profile.dateOfBirth"),
                                    "string"
                                }),
                                new BsonDocument("$dateFromString", new BsonDocument
                                {
                                    { "dateString", "$profile.dateOfBirth" },
                                    { "format", "%Y-%m-%d" },
                                    { "onError", BsonNull.Value },
                                    { "onNull", BsonNull.Value }
                                }),
                                "$profile.dateOfBirth"
                            })
                        }
                    }),

                    // Filter valid dates only
                    new BsonDocument("$match", new BsonDocument("parsedDob", new BsonDocument
                    {
                        { "$ne", BsonNull.Value }
                    })),

                    // Filter by age range
                    new BsonDocument("$match", new BsonDocument("parsedDob", new BsonDocument
                    {
                        { "$gte", new BsonDateTime(startDob) },
                        { "$lte", new BsonDateTime(endDob) }
                    })),

                    // Count results
                    new BsonDocument("$count", "count")
                };

                    try
                    {
                        var docs = await _collection.Aggregate<BsonDocument>(pipeline).ToListAsync(cancellationToken);
                        var count = docs.FirstOrDefault()?["count"]?.ToInt32() ?? 0;

                        _logger.LogInformation("Age group {AgeRange} count: {Count}", ageLabels[i], count);
                        ageStats[ageLabels[i]] = count;
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex, "Error processing age range {AgeRange}", ageLabels[i]);
                        ageStats[ageLabels[i]] = 0;
                    }
                }

                result["age"] = ageStats;
            }

            // ==== 2. HOMETOWN ====
            // (Keep original implementation as it doesn't depend on dateOfBirth)
            if (groupBy.ToLower() is "all" or "hometown")
            {
                var pipeline = new List<BsonDocument>
            {
                new BsonDocument("$match", new BsonDocument("profile.address", new BsonDocument("$exists", true))),
                new BsonDocument("$addFields", new BsonDocument
                {
                    { "province", new BsonDocument("$arrayElemAt", new BsonArray
                        {
                            new BsonDocument("$split", new BsonArray { "$profile.address", ", " }), -1
                        })
                    },
                    { "district", new BsonDocument("$arrayElemAt", new BsonArray
                        {
                            new BsonDocument("$split", new BsonArray { "$profile.address", ", " }), -2
                        })
                    }
                }),
                new BsonDocument("$group", new BsonDocument
                {
                    { "_id", new BsonDocument { { "province", "$province" }, { "district", "$district" } } },
                    { "count", new BsonDocument("$sum", 1) }
                }),
                new BsonDocument("$project", new BsonDocument
                {
                    { "_id", new BsonDocument("$concat", new BsonArray { "$_id.province", " - ", "$_id.district" }) },
                    { "count", 1 }
                })
            };

                var docs = await _collection.Aggregate<BsonDocument>(pipeline, cancellationToken: cancellationToken).ToListAsync(cancellationToken);
                result["hometown"] = docs.ToDictionary(x => x["_id"].AsString, x => x["count"].AsInt32);
            }

            // ==== 3. GENDER ====
            // (Keep original implementation)
            if (groupBy.ToLower() is "all" or "gender")
            {
                var pipeline = new List<BsonDocument>
            {
                new BsonDocument("$match", new BsonDocument("profile.gender", new BsonDocument("$exists", true))),
                new BsonDocument("$group", new BsonDocument
                {
                    { "_id", "$profile.gender" },
                    { "count", new BsonDocument("$sum", 1) }
                })
            };

                var docs = await _collection.Aggregate<BsonDocument>(pipeline, cancellationToken: cancellationToken).ToListAsync(cancellationToken);
                result["gender"] = docs.ToDictionary(x => x["_id"].AsString, x => x["count"].AsInt32);
            }

            // ==== 4. STATUS ====
            // (Keep original implementation)
            if (groupBy.ToLower() is "all" or "status")
            {
                var activeCount = await _collection.CountDocumentsAsync(
                    Builders<User>.Filter.Where(u => u.Status && !u.IsForbidden), cancellationToken: cancellationToken);
                var inactiveCount = await _collection.CountDocumentsAsync(
                    Builders<User>.Filter.Where(u => !u.Status && !u.IsForbidden), cancellationToken: cancellationToken);
                var forbiddenCount = await _collection.CountDocumentsAsync(
                    Builders<User>.Filter.Where(u => u.IsForbidden), cancellationToken: cancellationToken);

                result["status"] = new Dictionary<string, int>
            {
                { "Active", (int)activeCount },
                { "Inactive", (int)inactiveCount },
                { "Forbidden", (int)forbiddenCount }
            };
            }

            // ==== 5. CREATED TIME ====
            // (Keep original implementation)
            if (groupBy.ToLower() is "all" or "time")
            {
                var dateFormat = timeRange.ToLower() switch
                {
                    "day" => "%Y-%m-%d %H:00",
                    "week" or "month" => "%Y-%m-%d",
                    "year" => "%Y-%m",
                    _ => "%Y-%m-%d"
                };

                var pipeline = new List<BsonDocument>
            {
                new BsonDocument("$match", new BsonDocument("createdAt", new BsonDocument("$gte", new BsonDateTime(startDate)))),
                new BsonDocument("$group", new BsonDocument
                {
                    { "_id", new BsonDocument("$dateToString", new BsonDocument
                        {
                            { "format", dateFormat },
                            { "date", "$createdAt" }
                        })
                    },
                    { "count", new BsonDocument("$sum", 1) }
                }),
                new BsonDocument("$sort", new BsonDocument("_id", 1))
            };

                var docs = await _collection.Aggregate<BsonDocument>(pipeline, cancellationToken: cancellationToken).ToListAsync(cancellationToken);
                result["time"] = docs.ToDictionary(x => x["_id"].AsString, x => x["count"].AsInt32);
            }

            return result;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error fetching user statistics with groupBy={GroupBy} and timeRange={TimeRange}", groupBy, timeRange);
            throw;
        }
    }

    public async Task<bool> RestoreUser(string id, CancellationToken cancellationToken = default)
    {
        var filter = Builders<User>.Filter.Eq(u => u.Id, id);
        var update = Builders<User>.Update
            .Set(u => u.IsForbidden, false)
            .Set(u => u.Status, true)
            .Set(u => u.UpdatedAt, DateTime.UtcNow);

        var result = await _collection.UpdateOneAsync(filter, update, cancellationToken: cancellationToken);
        return result.ModifiedCount > 0;
    }
}
