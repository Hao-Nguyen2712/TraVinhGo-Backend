// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using MongoDB.Bson;
using MongoDB.Driver;
using TraVinhMaps.Application.UnitOfWorks;
using TraVinhMaps.Domain.Entities;
using TraVinhMaps.Domain.Specs;
using TraVinhMaps.Infrastructure.Db;

namespace TraVinhMaps.Infrastructure.UnitOfWork;
public class TouristDestinationRepository : Repository<TouristDestination>, ITouristDestinationRepository 
{
    public TouristDestinationRepository(IDbContext context) : base(context)
    {
    }

    public async Task<string> AddDestinationHistoryStoryImage(string id, string imageUrl, CancellationToken cancellationToken = default)
    {
        var filter = Builders<TouristDestination>.Filter.Eq(p => p.Id, id);

        var destination = await _collection.Find(filter).FirstOrDefaultAsync(cancellationToken);
        if (destination == null)
        {
            return null;
        }
        if (destination.HistoryStory.Images == null)
        {
            var setImagesUpdate = Builders<TouristDestination>.Update.Set(p => p.HistoryStory.Images, new List<String>());
            await _collection.UpdateOneAsync(filter, setImagesUpdate);
        }
        var pushImageUpdate = Builders<TouristDestination>.Update.Push(p => p.HistoryStory.Images, imageUrl);
        var updateResult = await _collection.UpdateOneAsync(filter, pushImageUpdate, cancellationToken: cancellationToken);
        return imageUrl;
    }

    public async Task<string> AddDestinationImage(string id, string imageUrl, CancellationToken cancellationToken = default)
    {
        var filter = Builders<TouristDestination>.Filter.Eq(p => p.Id, id);

        var destination = await _collection.Find(filter).FirstOrDefaultAsync(cancellationToken);
        if (destination == null) return null;
        if (destination.Images == null)
        {
            var setImagesUpdate = Builders<TouristDestination>.Update.Set(p => p.Images, new List<String>());
            await _collection.UpdateOneAsync(filter, setImagesUpdate);
        }
        var pushImageUpdate = Builders<TouristDestination>.Update.Push(p => p.Images, imageUrl);
        var updateResult = await _collection.UpdateOneAsync(filter, pushImageUpdate, cancellationToken: cancellationToken);
        return imageUrl;
    }

    public async Task<string> DeleteDestinationHistoryStoryImage(string id, string imageUrl, CancellationToken cancellationToken = default)
    {
        var filter = Builders<TouristDestination>.Filter.Eq(p => p.Id, id);

        var destination = await _collection.Find(filter).FirstOrDefaultAsync(cancellationToken);
        if (destination == null || destination.HistoryStory.Images == null || !destination.HistoryStory.Images.Contains(imageUrl))
        {
            return null;
        }

        var pullImageUpdate = Builders<TouristDestination>.Update.Pull(p => p.HistoryStory.Images, imageUrl);
        var updateResult = await _collection.UpdateOneAsync(filter, pullImageUpdate, cancellationToken: cancellationToken);
        return "Image deleted successfully";
    }

    public async Task<string> DeleteDestinationImage(string id, string imageUrl, CancellationToken cancellationToken = default)
    {
        var filter = Builders<TouristDestination>.Filter.Eq(p => p.Id, id);

        var destination = await _collection.Find(filter).FirstOrDefaultAsync(cancellationToken);
        if (destination == null || destination.Images == null || !destination.Images.Contains(imageUrl))
        {
            return null;
        }

        var pullImageUpdate = Builders<TouristDestination>.Update.Pull(p => p.Images, imageUrl);
        var updateResult = await _collection.UpdateOneAsync(filter, pullImageUpdate, cancellationToken: cancellationToken);
        return "Image deleted successfully";
    }

    public async Task<IEnumerable<TouristDestination>> GetByTagIdAsync(string tagId, CancellationToken cancellationToken = default)
    {
        FilterDefinition<TouristDestination> filter = Builders<TouristDestination>.Filter.Eq(p => p.TagId, tagId);
        return await _collection.Find(filter).ToListAsync(cancellationToken);
    }

    public async Task<Pagination<TouristDestination>> GetTouristDestination(TouristDestinationSpecParams touristDestinationSpecParams, CancellationToken cancellationToken = default)
    {
        var builder = Builders<TouristDestination>.Filter;
        var filter = builder.Eq(x => x.status ,true);
        if (!string.IsNullOrEmpty(touristDestinationSpecParams.Search))
        {
            var searchFilter = builder.Regex(x => x.Name, new BsonRegularExpression(touristDestinationSpecParams.Search, "i"));
            filter &= searchFilter;
        }

        if (!string.IsNullOrEmpty(touristDestinationSpecParams.Sort))
        {
            return new Pagination<TouristDestination>
            {
                PageSize = touristDestinationSpecParams.PageSize,
                PageIndex = touristDestinationSpecParams.PageIndex,
                Data = await DataFilter(touristDestinationSpecParams, filter, cancellationToken),
                Count = await _collection.CountDocumentsAsync(Builders<TouristDestination>.Filter.Empty, cancellationToken: cancellationToken)
            };
        }
        return new Pagination<TouristDestination>
            {
                PageSize = touristDestinationSpecParams.PageSize,
                PageIndex = touristDestinationSpecParams.PageIndex,
                Data = await _collection
                    .Find(filter)
                    .Sort(Builders<TouristDestination>.Sort.Ascending("Name"))
                    .Skip(touristDestinationSpecParams.PageSize * (touristDestinationSpecParams.PageIndex - 1))
                    .Limit(touristDestinationSpecParams.PageSize)
                    .ToListAsync(cancellationToken),
                Count = await _collection.CountDocumentsAsync(filter, cancellationToken: cancellationToken)
            };
    }

    public async Task<bool> PlusFavorite(string id, CancellationToken cancellationToken = default)
    {
        var filter = Builders<TouristDestination>.Filter.Eq(p => p.Id, id);
        var update = Builders<TouristDestination>.Update.Inc(p => p.FavoriteCount, 1);

        var result = await _collection.UpdateOneAsync(filter, update, cancellationToken: cancellationToken);

        return result.ModifiedCount > 0;
    }

    private async Task<IReadOnlyList<TouristDestination>> DataFilter(TouristDestinationSpecParams catalogSpecParams, FilterDefinition<TouristDestination> filter, CancellationToken cancellationToken = default)
    {
        switch (catalogSpecParams.Sort)
        {
            // favorite
            case "favoriteAsc":
                return await _collection
                    .Find(filter)
                    .Sort(Builders<TouristDestination>.Sort.Ascending("favoriteCount"))
                    .Skip(catalogSpecParams.PageSize * (catalogSpecParams.PageIndex - 1))
                    .Limit(catalogSpecParams.PageSize)
                    .ToListAsync(cancellationToken);
            case "favoriteDesc":
                return await _collection
                    .Find(filter)
                    .Sort(Builders<TouristDestination>.Sort.Descending("favoriteCount"))
                    .Skip(catalogSpecParams.PageSize * (catalogSpecParams.PageIndex - 1))
                    .Limit(catalogSpecParams.PageSize)
                    .ToListAsync(cancellationToken);
            // rating
            case "ratingAsc":
                return await _collection
                    .Find(filter)
                    .Sort(Builders<TouristDestination>.Sort.Ascending("avarageRating"))
                    .Skip(catalogSpecParams.PageSize * (catalogSpecParams.PageIndex - 1))
                    .Limit(catalogSpecParams.PageSize)
                    .ToListAsync(cancellationToken);
            case "ratingDesc":
                return await _collection
                    .Find(filter)
                    .Sort(Builders<TouristDestination>.Sort.Descending("avarageRating"))
                    .Skip(catalogSpecParams.PageSize * (catalogSpecParams.PageIndex - 1))
                    .Limit(catalogSpecParams.PageSize)
                    .ToListAsync(cancellationToken);
            // name
            case "nameAsc":
                return await _collection
                    .Find(filter)
                    .Sort(Builders<TouristDestination>.Sort.Ascending("name"))
                    .Skip(catalogSpecParams.PageSize * (catalogSpecParams.PageIndex - 1))
                    .Limit(catalogSpecParams.PageSize)
                    .ToListAsync(cancellationToken);
            case "nameDesc":
                return await _collection
                    .Find(filter)
                    .Sort(Builders<TouristDestination>.Sort.Descending("name"))
                    .Skip(catalogSpecParams.PageSize * (catalogSpecParams.PageIndex - 1))
                    .Limit(catalogSpecParams.PageSize)
                    .ToListAsync(cancellationToken);
            default:
                return await _collection
                    .Find(filter)
                    .Skip(catalogSpecParams.PageSize * (catalogSpecParams.PageIndex - 1))
                    .Limit(catalogSpecParams.PageSize)
                    .ToListAsync(cancellationToken);
        }
    }
}
