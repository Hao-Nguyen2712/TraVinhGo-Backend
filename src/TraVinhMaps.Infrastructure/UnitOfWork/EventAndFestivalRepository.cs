// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MongoDB.Driver;
using TraVinhMaps.Application.UnitOfWorks;
using TraVinhMaps.Domain.Entities;
using TraVinhMaps.Infrastructure.Db;

namespace TraVinhMaps.Infrastructure.UnitOfWork;
public class EventAndFestivalRepository : Repository<EventAndFestival>, IEventAndFestivalRepository
{
    public EventAndFestivalRepository(IDbContext context) : base(context)
    {
    }

    public async Task<string> AddEventAndFestivalImage(string id, string imageUrl, CancellationToken cancellationToken = default)
    {
        var filter = Builders<EventAndFestival>.Filter.Eq(p => p.Id, id);

        var eventAndFestinal = await _collection.Find(filter).FirstOrDefaultAsync(cancellationToken);
        if (eventAndFestinal == null) return null;
        if (eventAndFestinal.Images == null)
        {
            var setImagesUpdate = Builders<EventAndFestival>.Update.Set(p => p.Images, new List<String>());
            await _collection.UpdateOneAsync(filter, setImagesUpdate);
        }
        var pushImageUpdate = Builders<EventAndFestival>.Update.Push(p => p.Images, imageUrl);
        var updateResult = await _collection.UpdateOneAsync(filter, pushImageUpdate, cancellationToken: cancellationToken);
        return imageUrl;
    }

    public async Task<string> DeleteEventAndFestivalImage(string id, string imageUrl, CancellationToken cancellationToken = default)
    {
        var filter = Builders<EventAndFestival>.Filter.Eq(p => p.Id, id);

        var eventAndFestinal = await _collection.Find(filter).FirstOrDefaultAsync(cancellationToken);
        if (eventAndFestinal == null || eventAndFestinal.Images == null || !eventAndFestinal.Images.Contains(imageUrl))
        {
            return null;
        }

        var pullImageUpdate = Builders<EventAndFestival>.Update.Pull(p => p.Images, imageUrl);
        var updateResult = await _collection.UpdateOneAsync(filter, pullImageUpdate, cancellationToken: cancellationToken);
        return "Image deleted successfully";
    }
}
