using MongoDB.Bson;
using MongoDB.Driver;
using TraVinhMaps.Application.UnitOfWorks;
using TraVinhMaps.Domain.Entities;
using TraVinhMaps.Infrastructure.Db;
using Microsoft.Extensions.Logging;
using DnsClient.Internal;
using TraVinhMaps.Application.Common.Exceptions;

namespace TraVinhMaps.Infrastructure.CustomRepositories;

public class UserRepository : BaseRepository<User>, IUserRepository
{
    private readonly IMongoDatabase _database;
    private readonly ILogger<UserRepository> _logger;

    public UserRepository(IDbContext context, ILogger<UserRepository> logger) : base(context)
    {
        _database = context.Database;
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

    public async Task<Dictionary<string, Dictionary<string, int>>> GetPerformanceByTagAsync(
    IEnumerable<string>? tagNames,
    bool includeOcop,
    bool includeDestination,
    bool includeLocalSpecialty,
    bool includeTips,
    bool includeFestivals,
    string timeRange = "month",
    DateTime? startDate = null,
    DateTime? endDate = null,
    CancellationToken cancellationToken = default)
    {
        try
        {
            startDate ??= DateTime.UtcNow.AddHours(7).AddDays(-30).Date;
            endDate ??= DateTime.UtcNow.AddHours(7).Date.AddDays(1);

            var tagCol = _database.GetCollection<Tags>("Tags");
            var tagFilter = tagNames == null || !tagNames.Any()
                ? FilterDefinition<Tags>.Empty
                : Builders<Tags>.Filter.In(t => t.Name, tagNames);
            var tagDocs = await tagCol.Find(tagFilter).ToListAsync(cancellationToken);

            var tagIdToName = tagDocs.ToDictionary(
                t => ObjectId.Parse(t.Id),
                t => t.Name);

            if (!tagDocs.Any() && tagNames != null && tagNames.Any())
            {
                tagIdToName = tagNames.ToDictionary(t => ObjectId.GenerateNewId(), t => t);
                _logger.LogWarning("No tags found in the database, using provided tagNames.");
            }
            else if (!tagDocs.Any())
            {
                _logger.LogWarning("No tags found, returning empty result.");
                return new Dictionary<string, Dictionary<string, int>>();
            }

            _logger.LogDebug("⮞ Processing {Count} tag(s)", tagIdToName.Count);

            var config = new[]
            {
            (includeOcop, "OcopProduct"),
            (includeDestination, "TouristDestination"),
            (includeLocalSpecialty, "LocalSpecialties"),
            (includeTips, "Tips"),
            (includeFestivals, "EventAndFestival")
        };

            var itemMap = new Dictionary<ObjectId, string>();

            foreach (var (isOn, colName) in config)
            {
                if (!isOn) continue;

                var col = _database.GetCollection<BsonDocument>(colName);
                var tagIdArray = new BsonArray(tagIdToName.Keys.Select(id => (BsonValue)id));
                var tagIdStrArray = new BsonArray(tagIdToName.Keys.Select(id => (BsonValue)id.ToString()));

                var filter = Builders<BsonDocument>.Filter.Or(
                    Builders<BsonDocument>.Filter.In("tagId", tagIdArray),
                    Builders<BsonDocument>.Filter.In("tagId", tagIdStrArray),
                    Builders<BsonDocument>.Filter.AnyIn("tags", tagIdArray),
                    Builders<BsonDocument>.Filter.AnyIn("tags", tagIdStrArray)
                );

                var logDocs = await col.Find(filter)
                    .Project(Builders<BsonDocument>.Projection.Include("_id").Include("tagId").Include("tags"))
                    .ToListAsync(cancellationToken);

                _logger.LogDebug("▶ {Collection}: {Count} items matched", colName, logDocs.Count);

                foreach (var doc in logDocs)
                {
                    var itemId = doc["_id"].AsObjectId;
                    ObjectId? matchedTagId = null;

                    if (doc.TryGetValue("tagId", out var tIdVal))
                    {
                        if (tIdVal.IsObjectId && tagIdToName.ContainsKey(tIdVal.AsObjectId))
                            matchedTagId = tIdVal.AsObjectId;
                        else if (tIdVal.IsString && ObjectId.TryParse(tIdVal.AsString, out var oid) && tagIdToName.ContainsKey(oid))
                            matchedTagId = oid;
                    }
                    else if (doc.TryGetValue("tags", out var tagsVal) && tagsVal.IsBsonArray)
                    {
                        foreach (var tagVal in tagsVal.AsBsonArray)
                        {
                            if (tagVal.IsObjectId && tagIdToName.ContainsKey(tagVal.AsObjectId))
                            {
                                matchedTagId = tagVal.AsObjectId;
                                break;
                            }
                            if (tagVal.IsString && ObjectId.TryParse(tagVal.AsString, out var oid) && tagIdToName.ContainsKey(oid))
                            {
                                matchedTagId = oid;
                                break;
                            }
                        }
                    }

                    if (matchedTagId.HasValue)
                    {
                        var tagName = tagIdToName[matchedTagId.Value];
                        itemMap[itemId] = tagName;
                    }
                }
            }

            var logs = _database.GetCollection<BsonDocument>("InteractionLogs");

            var match = new BsonDocument("$match", new BsonDocument
        {
            { "itemId", new BsonDocument("$in", new BsonArray(itemMap.Keys)) },
            { "createdAt", new BsonDocument {
                { "$gte", new BsonDateTime(startDate.Value) },
                { "$lte", new BsonDateTime(endDate.Value) }
            }}
        });

            string format = timeRange.ToLower() switch
            {
                "week" => "%G-%V",
                "month" => "%Y-%m",
                "year" => "%Y",
                _ => "%Y-%m-%d"
            };

            var project = new BsonDocument("$project", new BsonDocument
        {
            { "itemId", 1 },
            { "period", new BsonDocument("$dateToString", new BsonDocument
                {
                    { "format", format },
                    { "date", "$createdAt" },
                    { "timezone", "Asia/Ho_Chi_Minh" }
                })
            }
        });

            var docs = await logs.Aggregate<BsonDocument>(new[] { match, project }).ToListAsync(cancellationToken);

            _logger.LogDebug("⮞ InteractionLogs matched: {Count}", docs.Count);

            var result = new Dictionary<string, Dictionary<string, int>>();

            foreach (var doc in docs)
            {
                var itemId = doc["itemId"].AsObjectId;
                var period = doc["period"].AsString;

                if (!itemMap.TryGetValue(itemId, out var tagName)) continue;

                if (!result.ContainsKey(tagName)) result[tagName] = new();
                if (!result[tagName].ContainsKey(period)) result[tagName][period] = 0;

                result[tagName][period] += 1;
            }

            var targetTags = tagNames != null && tagNames.Any() ? tagNames : tagIdToName.Values;

            var tz = TimeZoneInfo.FindSystemTimeZoneById("SE Asia Standard Time");
            var localStart = TimeZoneInfo.ConvertTimeFromUtc(startDate.Value, tz);
            var localEnd = TimeZoneInfo.ConvertTimeFromUtc(endDate.Value, tz);

            var expectedPeriods = new List<string>();

            switch (timeRange.ToLower())
            {
                case "week":
                    for (var dt = localStart.Date; dt <= localEnd.Date; dt = dt.AddDays(7))
                    {
                        var isoWeek = System.Globalization.ISOWeek.GetWeekOfYear(dt);
                        var year = System.Globalization.ISOWeek.GetYear(dt);
                        expectedPeriods.Add($"{year}-{isoWeek:D2}");
                    }
                    break;
                case "month":
                    for (var dt = new DateTime(localStart.Year, localStart.Month, 1); dt <= localEnd; dt = dt.AddMonths(1))
                    {
                        expectedPeriods.Add(dt.ToString("yyyy-MM"));
                    }
                    break;
                case "year":
                    for (var y = localStart.Year; y <= localEnd.Year; y++)
                    {
                        expectedPeriods.Add(y.ToString());
                    }
                    break;
                case "day":
                default:
                    expectedPeriods.Add(localStart.ToString("yyyy-MM-dd"));
                    break;
            }

            foreach (var tag in targetTags)
            {
                if (!result.ContainsKey(tag)) result[tag] = new();
                foreach (var period in expectedPeriods)
                {
                    if (!result[tag].ContainsKey(period)) result[tag][period] = 0;
                }
            }

            _logger.LogInformation("GetPerformanceByTagsAsync completed – returning {TagCount} tags", result.Count);
            return result;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "GetPerformanceByTagsAsync failed");
            throw;
        }
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
            var now = DateTime.UtcNow;
            var nowInVietnam = now.AddHours(7);
            var result = new Dictionary<string, object>();

            var ageGroups = new[] { 0, 18, 30, 50, 120 };
            var ageLabels = new[] { "0-18", "18-30", "30-50", "50+" };

            DateTime startDate, endDate;
            switch (timeRange.ToLower())
            {
                case "day":
                    startDate = nowInVietnam.Date;
                    endDate = startDate.AddDays(1);
                    break;
                case "week":
                    int diff = (7 + (int)nowInVietnam.DayOfWeek - 1) % 7;
                    startDate = nowInVietnam.Date.AddDays(-diff);
                    endDate = startDate.AddDays(7);
                    break;
                case "month":
                    startDate = new DateTime(nowInVietnam.Year, nowInVietnam.Month, 1);
                    endDate = startDate.AddMonths(1);
                    break;
                case "year":
                    startDate = new DateTime(nowInVietnam.Year, 1, 1);
                    endDate = startDate.AddYears(1);
                    break;
                default:
                    throw new ArgumentException($"Invalid timeRange: {timeRange}");
            }

            var startDateUtc = startDate.AddHours(-7);
            var endDateUtc = endDate.AddHours(-7);

            _logger.LogInformation("Time range {TimeRange}: startDate={StartDate} (+07), endDate={EndDate} (+07), startDateUtc={StartDateUtc}, endDateUtc={EndDateUtc}",
                timeRange, startDate, endDate, startDateUtc, endDateUtc);

            var createdAtFilter = new BsonDocument("createdAt", new BsonDocument
        {
            { "$gte", new BsonDateTime(startDateUtc) },
            { "$lt", new BsonDateTime(endDateUtc) }
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

                    var pipeline = new List<BsonDocument>
                {
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
                        { "$ne", BsonNull.Value },
                        { "$gte", new BsonDateTime(startDob) },
                        { "$lte", new BsonDateTime(endDob) }
                    })),
                    new BsonDocument("$match", createdAtFilter),
                    new BsonDocument("$count", "count")
                };

                    try
                    {
                        var docs = await _collection.Aggregate<BsonDocument>(pipeline).ToListAsync(cancellationToken);
                        var count = docs.FirstOrDefault()?["count"]?.ToInt32() ?? 0;
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
                new BsonDocument("$match", new BsonDocument {
                    { "profile.address", new BsonDocument("$exists", true) },
                    { "createdAt", createdAtFilter["createdAt"] }
                }),
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
                new BsonDocument("$match", new BsonDocument {
                    { "profile.gender", new BsonDocument("$exists", true) },
                    { "createdAt", createdAtFilter["createdAt"] }
                }),
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
                    Builders<User>.Filter.Where(u => u.Status && !u.IsForbidden && u.CreatedAt >= startDateUtc && u.CreatedAt < endDateUtc),
                    cancellationToken: cancellationToken);
                var inactiveCount = await _collection.CountDocumentsAsync(
                    Builders<User>.Filter.Where(u => !u.Status && !u.IsForbidden && u.CreatedAt >= startDateUtc && u.CreatedAt < endDateUtc),
                    cancellationToken: cancellationToken);
                var forbiddenCount = await _collection.CountDocumentsAsync(
                    Builders<User>.Filter.Where(u => u.IsForbidden && u.UpdatedAt >= startDateUtc && u.UpdatedAt < endDateUtc),
                    cancellationToken: cancellationToken);

                result["status"] = new Dictionary<string, int>
            {
                { "Active", (int)activeCount },
                { "Inactive", (int)inactiveCount },
                { "Forbidden", (int)forbiddenCount }
            };
            }

            // ==== 5. TIME ====
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
                new BsonDocument("$match", createdAtFilter),
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
