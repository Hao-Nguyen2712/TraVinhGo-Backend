// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DnsClient;
using System.Xml.Linq;
using MongoDB.Driver;
using TraVinhMaps.Application.Features.Review.Models;
using TraVinhMaps.Application.Repositories;
using TraVinhMaps.Domain.Entities;
using TraVinhMaps.Infrastructure.CustomRepositories;
using TraVinhMaps.Infrastructure.Db;
using static System.Net.Mime.MediaTypeNames;
using MongoDB.Bson;

namespace TraVinhMaps.Infrastructure.Repositories;
public class ReviewRepository : BaseRepository<Review>, IReviewRepository
{
    private readonly IMongoCollection<User> _userCollection;
    public ReviewRepository(IDbContext dbContext) : base(dbContext)
    {

        _userCollection = dbContext.GetCollection<User>();
    }
    public async Task<IEnumerable<ReviewResponse>> FilterReviewsAsync(string? destinationId, int? rating, DateTime? startAt, DateTime? endAt, CancellationToken cancellationToken = default)
    {
        var filters = new List<FilterDefinition<Review>>();

        if (!string.IsNullOrEmpty(destinationId))
            filters.Add(Builders<Review>.Filter.Eq(r => r.DestinationId, destinationId));

        if (rating.HasValue)
            filters.Add(Builders<Review>.Filter.Eq(r => r.Rating, rating.Value));

        if (startAt.HasValue && endAt.HasValue)
        {
            var startDate = DateTime.SpecifyKind(startAt.Value.Date, DateTimeKind.Utc);
            var endDate = DateTime.SpecifyKind(endAt.Value.Date.AddDays(1).AddTicks(-1), DateTimeKind.Utc);

            filters.Add(Builders<Review>.Filter.Gte(r => r.CreatedAt, startDate));
            filters.Add(Builders<Review>.Filter.Lte(r => r.CreatedAt, endDate));
        }

        else if (startAt.HasValue)
        {
            var startDate = startAt.Value.Date;
            filters.Add(Builders<Review>.Filter.Gte(r => r.CreatedAt, startDate));
        }
        else if (endAt.HasValue)
        {
            var endDate = endAt.Value.Date.AddDays(1).AddTicks(-1);
            filters.Add(Builders<Review>.Filter.Lte(r => r.CreatedAt, endDate));
        }

        var filter = filters.Any() ? Builders<Review>.Filter.And(filters) : Builders<Review>.Filter.Empty;

        var reviews = await _collection.Find(filter).ToListAsync(cancellationToken);

        var responses = reviews.Select(r => new ReviewResponse
        {
            Id = r.Id,
            Rating = r.Rating,
            Images = r.Images,
            Comment = r.Comment,
            UserId = r.UserId,
            DestinationId = r.DestinationId,
            CreatedAt = r.CreatedAt,
            Reply = r.Reply
        }).ToList();

        return responses;
    }

    public async Task<string> AddImageReview(string id, string imageUrl, CancellationToken cancellationToken = default)
    {
        var filter = Builders<Review>.Filter.Eq(o => o.Id, id);
        var reviewImage = await _collection.Find(filter).FirstOrDefaultAsync(cancellationToken);
        if (reviewImage == null) return null;
        if (reviewImage.Images == null)
        {
            var newListImage = Builders<Review>.Update.Set(im => im.Images, new List<string>());
            await _collection.UpdateOneAsync(filter, newListImage);
        }
        var updateImage = Builders<Review>.Update.Push(p => p.Images, imageUrl);
        var updateResult = await _collection.UpdateOneAsync(filter, updateImage, cancellationToken: cancellationToken);
        return imageUrl;
    }

    public async Task<Reply> AddReply(string id, Reply reply, CancellationToken cancellationToken = default)
    {
        var filter = Builders<Review>.Filter.Eq(r => r.Id, id);
        var review = await _collection.Find(filter).FirstOrDefaultAsync(cancellationToken);
        if (review == null) return null;
        if (review.Reply == null)
        {
            var newReply = Builders<Review>.Update.Set(im => im.Reply, new List<Reply>());
            await _collection.UpdateOneAsync(filter, newReply);
        }
        var updateReply = Builders<Review>.Update.Push(re => re.Reply, reply);
        var updateResult = await _collection.UpdateOneAsync(filter, updateReply, cancellationToken: cancellationToken);
        return reply;
    }

    public async Task<ReviewResponse> GetReviewByIdAsync(string id, CancellationToken cancellationToken)
    {
        var filter = Builders<Review>.Filter.Eq(r => r.Id, id);
        var review = await _collection.Find(filter).FirstOrDefaultAsync(cancellationToken);
        if (review == null)
            return null;

        var response = new ReviewResponse
        {
            Id = review.Id,
            Rating = review.Rating,
            Images = review.Images,
            Comment = review.Comment,
            UserId = review.UserId,
            DestinationId = review.DestinationId,
            CreatedAt = review.CreatedAt,
            Reply = review.Reply
        };

        return response;
    }

    public async Task<IEnumerable<ReviewResponse>> GetLatestReviewsAsync(int count = 5, CancellationToken cancellationToken = default)
    {
        var reviews = await _collection.Find(Builders<Review>.Filter.Empty).SortByDescending(r => r.CreatedAt).Limit(count).ToListAsync(cancellationToken);
        var responses = reviews.Select(r => new ReviewResponse
        {
            Id = r.Id,
            Rating = r.Rating,
            Images = r.Images,
            Comment = r.Comment,
            UserId = r.UserId,
            DestinationId = r.DestinationId,
            CreatedAt = r.CreatedAt,
            Reply = r.Reply
        });
        return responses;
    }

    public async Task<long> GetTotalUsersReviewedAsync(CancellationToken cancellationToken = default)
    {
        var totalUser = await _collection.Distinct<string>(
            nameof(Review.UserId),
            Builders<Review>.Filter.Empty,
            options: null,
            cancellationToken
        ).ToListAsync(cancellationToken);
        return totalUser.LongCount();
    }

    public async Task<long> GetTotalFiveStarReviewsAsync(CancellationToken cancellationToken = default)
    {
        var filter = Builders<Review>.Filter.Eq(r => r.Rating, 5);
        var count = await _collection.CountDocumentsAsync(filter, cancellationToken: cancellationToken);
        return count;
    }

    public async Task<(string UserId, long ReviewCount)> GetTopReviewerAsync(CancellationToken cancellationToken = default)
    {
        var currentDate = DateTime.UtcNow;
        var year = currentDate.Year;
        var month = currentDate.Month;

        Console.WriteLine($"Debug: Target Year: {year}, Month: {month}");

        var matchExpr = new BsonDocument
    {
        {
            "$expr", new BsonDocument("$and", new BsonArray
            {
                new BsonDocument("$eq", new BsonArray { new BsonDocument("$year", "$createdAt"), year }),
                new BsonDocument("$eq", new BsonArray { new BsonDocument("$month", "$createdAt"), month })
            })
        }
    };

        var pipeline = new[]
        {
        new BsonDocument("$match", matchExpr),
        new BsonDocument("$group", new BsonDocument
        {
            { "_id", "$userId" },
            { "count", new BsonDocument("$sum", 1) }
        }),
        new BsonDocument("$sort", new BsonDocument("count", -1)),
        new BsonDocument("$limit", 1)
    };

        var results = await _collection.Aggregate<BsonDocument>(pipeline).ToListAsync(cancellationToken);
        var result = results.FirstOrDefault();

        if (result == null)
        {
            Console.WriteLine("Debug: No results found.");
            return (null, 0);
        }

        var userId = result["_id"].AsObjectId.ToString();
        var reviewCount = result["count"].ToInt64();
        var userFilter = Builders<User>.Filter.Eq(u => u.Id, userId);
        var userDoc = await _userCollection.Find(userFilter).FirstOrDefaultAsync(cancellationToken);

        string userName = userDoc?.Username ?? "Unknown";

        return (userName, reviewCount);
    }

    public async Task<IEnumerable<Review>> GetListReviewByUserId(string id, CancellationToken cancellationToken = default)
    {
        var filter = Builders<Review>.Filter.Eq(r => r.UserId, id);
        var reviews = await _collection.Find(filter).ToListAsync();
        return reviews;
    }

}
