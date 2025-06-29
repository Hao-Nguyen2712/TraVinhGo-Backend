// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using MongoDB.Driver;
using TraVinhMaps.Application.Repositories;
using TraVinhMaps.Domain.Entities;
using TraVinhMaps.Infrastructure.CustomRepositories;
using TraVinhMaps.Infrastructure.Db;

namespace TraVinhMaps.Infrastructure.Repositories;
public class OcopTypeRepository : BaseRepository<OcopType>, IOcopTypeRepository
{
    public OcopTypeRepository(IDbContext dbContext) : base(dbContext)
    {
        
    }
    public async Task<OcopType> GetOcopTypeByName(string name, CancellationToken cancellationToken = default)
    {
        var filter = Builders<OcopType>.Filter.Eq(t => t.OcopTypeName, name) & Builders<OcopType>.Filter.Eq(o => o.OcopTypeStatus, true);
        return await _collection.Find(filter).FirstOrDefaultAsync(cancellationToken);
    }
}
