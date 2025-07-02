using MongoDB.Bson;
using MongoDB.Driver;
using TraVinhMaps.Application.UnitOfWorks;
using TraVinhMaps.Domain.Entities;
using TraVinhMaps.Infrastructure.Db;
using Microsoft.Extensions.Logging;
using TraVinhMaps.Application.Common.Exceptions;

namespace TraVinhMaps.Infrastructure.CustomRepositories;

public class UserRepository : BaseRepository<User>, IUserRepository
{
    private readonly ILogger<UserRepository> _logger;

    public UserRepository(IDbContext context, ILogger<UserRepository> logger) : base(context)
    {
        _logger = logger;
    }

    public async Task<bool> addItemToFavoriteList(string id, Favorite favorite, CancellationToken cancellationToken = default)
    {
        var filter = Builders<User>.Filter.Eq(u => u.Id, id);
        var user = await _collection.Find(filter).FirstOrDefaultAsync(cancellationToken);
        if (user == null)
        {
            throw new NotFoundException($"User with id {id} not found.");
        }

        if (user.Favorites == null)
        {
            var setfavoriteUpdate = Builders<User>.Update.Set(p => p.Favorites, new List<Favorite>());
            await _collection.UpdateOneAsync(filter, setfavoriteUpdate);
        }
        var pushFavoriteUpdate = Builders<User>.Update.Push(p => p.Favorites, favorite);
        var updateResult = await _collection.UpdateOneAsync(filter, pushFavoriteUpdate, cancellationToken: cancellationToken);
        return true;
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

    public async Task<List<Favorite>> getFavoriteUserList(string id, CancellationToken cancellationToken = default)
    {
        var filter = Builders<User>.Filter.Eq(u => u.Id, id);
        var user = await _collection.Find(filter).FirstOrDefaultAsync(cancellationToken);
        if (user == null)
        {
            throw new NotFoundException($"User with id {id} not found.");
        }
        return user.Favorites ?? new List<Favorite>();
    }

    public Task<User> GetUserByEmailAsync(string email, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    public async Task<Dictionary<string, object>> GetUserStatisticsAsync(string groupBy, string timeRange, CancellationToken cancellationToken = default)
    {
        try
        {
            var now = DateTime.UtcNow; // Current time in UTC
            var result = new Dictionary<string, object>();

            // Adjust for UTC+7 (Vietnam time zone)
            var nowInVietnam = now.AddHours(7); // Convert UTC to UTC+7

            var ageGroups = new[] { 0, 18, 30, 50, 120 };
            var ageLabels = new[] { "0-18", "18-30", "30-50", "50+" };

            // Calculate startDate and endDate in UTC+7, then convert to UTC for MongoDB
            DateTime startDate, endDate;
            switch (timeRange.ToLower())
            {
                case "day":
                    startDate = nowInVietnam.Date; // Start of today in UTC+7
                    endDate = startDate.AddDays(1).AddTicks(-1); // End of today in UTC+7
                    break;
                case "week":
                    startDate = nowInVietnam.Date.AddDays(-(int)nowInVietnam.DayOfWeek);
                    endDate = startDate.AddDays(7).AddTicks(-1);
                    break;
                case "month":
                    startDate = new DateTime(nowInVietnam.Year, nowInVietnam.Month, 1);
                    endDate = startDate.AddMonths(1).AddTicks(-1);
                    break;
                case "year":
                    startDate = new DateTime(nowInVietnam.Year, 1, 1);
                    endDate = startDate.AddYears(1).AddTicks(-1);
                    break;
                default:
                    throw new ArgumentException($"Invalid timeRange: {timeRange}");
            }

            // Convert startDate and endDate to UTC for MongoDB query (assuming createdAt is stored in UTC)
            var startDateUtc = startDate.AddHours(-7);
            var endDateUtc = endDate.AddHours(-7);

            _logger.LogInformation("Time range {TimeRange}: startDate={StartDate} (+07), endDate={EndDate} (+07), startDateUtc={StartDateUtc}, endDateUtc={EndDateUtc}",
                timeRange, startDate, endDate, startDateUtc, endDateUtc);

            // Common match stage for createdAt (used for age, hometown, gender, time)
            var timeFilter = new BsonDocument("$match", new BsonDocument
        {
            { "createdAt", new BsonDocument
                {
                    { "$gte", new BsonDateTime(startDateUtc) },
                    { "$lte", new BsonDateTime(endDateUtc) }
                }
            }
        });

            // ==== 1. AGE ====
            if (groupBy.ToLower() is "all" or "age")
            {
                var ageStats = new Dictionary<string, int>();

                for (int i = 0; i < ageLabels.Length; i++)
                {
                    var minAge = ageGroups[i];
                    var maxAge = ageGroups[i + 1];

                    var startDob = nowInVietnam.AddYears(-maxAge).Date;
                    var endDob = nowInVietnam.AddYears(-minAge).Date.AddDays(1).AddTicks(-1);

                    _logger.LogInformation("Processing age range {AgeRange}: startDob={StartDob}, endDob={EndDob}",
                        ageLabels[i], startDob, endDob);

                    var pipeline = new List<BsonDocument>
                {
                    timeFilter,
                    new BsonDocument("$match", new BsonDocument("profile.dateOfBirth", new BsonDocument("$exists", true))),
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
                    new BsonDocument("$match", new BsonDocument("parsedDob", new BsonDocument
                    {
                        { "$ne", BsonNull.Value }
                    })),
                    new BsonDocument("$match", new BsonDocument("parsedDob", new BsonDocument
                    {
                        { "$gte", new BsonDateTime(startDob) },
                        { "$lte", new BsonDateTime(endDob) }
                    })),
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
            if (groupBy.ToLower() is "all" or "hometown")
            {
                var pipeline = new List<BsonDocument>
            {
                timeFilter,
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
            if (groupBy.ToLower() is "all" or "gender")
            {
                var pipeline = new List<BsonDocument>
            {
                timeFilter,
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
            if (groupBy.ToLower() is "all" or "status")
            {
                var activeCount = await _collection.CountDocumentsAsync(
                    Builders<User>.Filter.Where(u => u.Status && !u.IsForbidden && u.CreatedAt >= startDateUtc), cancellationToken: cancellationToken);
                var inactiveCount = await _collection.CountDocumentsAsync(
                    Builders<User>.Filter.Where(u => !u.Status && !u.IsForbidden && u.CreatedAt >= startDateUtc), cancellationToken: cancellationToken);
                var forbiddenCount = await _collection.CountDocumentsAsync(
                    Builders<User>.Filter.Where(u => u.IsForbidden && u.UpdatedAt >= startDateUtc), cancellationToken: cancellationToken);

                result["status"] = new Dictionary<string, int>
            {
                { "Active", (int)activeCount },
                { "Inactive", (int)inactiveCount },
                { "Forbidden", (int)forbiddenCount }
            };
            }

            // ==== 5. CREATED TIME ====
            if (groupBy.ToLower() is "all" or "time")
            {
                var dateFormat = timeRange.ToLower() switch
                {
                    "day" => "%Y-%m-%d %H:00", // Group by hour for day
                    "week" or "month" => "%Y-%m-%d",
                    "year" => "%Y-%m",
                    _ => "%Y-%m-%d"
                };

                var pipeline = new List<BsonDocument>
            {
                timeFilter,
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
                _logger.LogInformation("Time range {TimeRange}: {Count} users created between {StartDate} and {EndDate}",
                    timeRange, docs.Sum(x => x["count"].AsInt32), startDate, endDate);
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

    public async Task<bool> removeItemToFavoriteList(string id, string itemId, CancellationToken cancellationToken = default)
    {
        var filter = Builders<User>.Filter.Eq(u => u.Id, id);
        var user = await _collection.Find(filter).FirstOrDefaultAsync(cancellationToken);
        if (user == null)
        {
            throw new NotFoundException($"User with id {id} not found.");
        }
        var update = Builders<User>.Update.PullFilter(u => u.Favorites,
        Builders<Favorite>.Filter.Eq(f => f.ItemId, itemId));

        var updateResult = await _collection.UpdateOneAsync(filter, update, cancellationToken: cancellationToken);
        return true;
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
