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
using TraVinhMaps.Infrastructure.UnitOfWork;

namespace TraVinhMaps.Infrastructure.CustomRepositories;
public class MarkerRepository : Repository<Marker>, IMarkerRepository
{
    public MarkerRepository(IDbContext context) : base(context)
    {
    }

    public async Task<string> AddMarkerImage(string id, string imageUrl, CancellationToken cancellationToken = default)
    {
        var filter = Builders<Marker>.Filter.Eq(p => p.Id, id);

        var marker = await _collection.Find(filter).FirstOrDefaultAsync(cancellationToken);
        if (marker == null) return null;
        var pushImageUpdate = Builders<Marker>.Update.Set(p => p.Image, imageUrl);
        var updateResult = await _collection.UpdateOneAsync(filter, pushImageUpdate, cancellationToken: cancellationToken);
        return imageUrl;
    }
}
