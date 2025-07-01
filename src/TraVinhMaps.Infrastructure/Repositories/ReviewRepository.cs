// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MongoDB.Driver;
using TraVinhMaps.Application.Repositories;
using TraVinhMaps.Domain.Entities;
using TraVinhMaps.Infrastructure.CustomRepositories;
using TraVinhMaps.Infrastructure.Db;

namespace TraVinhMaps.Infrastructure.Repositories;
public class ReviewRepository : BaseRepository<Review>, IReviewRepository
{
    public ReviewRepository(IDbContext dbContext) : base(dbContext) { }
    public async Task<IEnumerable<Review>> GetReviewsAsync(int rating, string destinationTypeId, CancellationToken cancellationToken = default)
    {
        var filter = Builders<Review>.Filter.And(Builders<Review>.Filter.Eq(r => r.Rating, rating),
                                                 Builders<Review>.Filter.Eq(r => r.DestinationId, destinationTypeId));
        var review = await _collection.Find(filter).ToListAsync();
        return review;
    }
    public async Task<string> AddImageReview(string id, string imageUrl, CancellationToken cancellationToken = default)
    {
        var filter = Builders<Review>.Filter.Eq(o => o.Id, id);
        var ocopProduct = await _collection.Find(filter).FirstOrDefaultAsync(cancellationToken);
        if (ocopProduct == null) return null;
        if (ocopProduct.Images == null)
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
}
