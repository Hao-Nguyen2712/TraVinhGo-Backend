// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using MongoDB.Driver;
using TraVinhMaps.Application.UnitOfWorks;
using TraVinhMaps.Domain.Entities;
using TraVinhMaps.Infrastructure.Db;

namespace TraVinhMaps.Infrastructure.UnitOfWork;
public class TouristDestinationRepository : Repository<TouristDestination>, ITouristDestinationRepository 
{
    public TouristDestinationRepository(IDbContext context) : base(context)
    {
    }

    public async Task<string> AddDestinationImage(string id, string imageUrl, CancellationToken cancellationToken = default)
    {
        var filter = Builders<TouristDestination>.Filter.Eq(p => p.Id, id);

        var destination = await _collection.Find(filter).FirstOrDefaultAsync(cancellationToken);
        if (destination == null) return null;
        if(destination.Images == null)
        {
            var setImagesUpdate = Builders<TouristDestination>.Update.Set(p => p.Images, new List<String>());
            await _collection.UpdateOneAsync(filter, setImagesUpdate);
        }
        var pushImageUpdate = Builders<TouristDestination>.Update.Push(p => p.Images, imageUrl);
        var updateResult = await _collection.UpdateOneAsync(filter, pushImageUpdate, cancellationToken: cancellationToken);
        return imageUrl;
    }

    public async Task<string> DeleteDestinationImage(string id, string imageUrl, CancellationToken cancellationToken = default)
    {
        var filter = Builders<TouristDestination>.Filter.Eq(p => p.Id, id);

        var destination = await _collection.Find(filter).FirstOrDefaultAsync(cancellationToken);
        if (destination == null || destination.Images == null || !destination.Images.Contains(imageUrl))
            return null;

        var pullImageUpdate = Builders<TouristDestination>.Update.Pull(p => p.Images, imageUrl);
        var updateResult = await _collection.UpdateOneAsync(filter, pullImageUpdate, cancellationToken: cancellationToken);
        return imageUrl;
    }

    public async Task<IEnumerable<TouristDestination>> GetByTagIdAsync(string tagId, CancellationToken cancellationToken = default)
    {
        FilterDefinition<TouristDestination> filter = Builders<TouristDestination>.Filter.Eq(p => p.TagId, tagId);
        return await _collection.Find(filter).ToListAsync();
    }
}
