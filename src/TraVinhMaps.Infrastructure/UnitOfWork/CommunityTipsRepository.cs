// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using MongoDB.Driver;
using TraVinhMaps.Application.UnitOfWorks;
using TraVinhMaps.Domain.Entities;
using TraVinhMaps.Infrastructure.Db;

namespace TraVinhMaps.Infrastructure.UnitOfWork;
public class CommunityTipsRepository : Repository<Tips>, ICommunityTipsRepository
{
    public CommunityTipsRepository(IDbContext dbContext) : base(dbContext) { }
    public async Task<IEnumerable<Tips>> GetAllTipActiveAsync(CancellationToken cancellationToken = default)
    {
        var filter = Builders<Tips>.Filter.Eq(o => o.Status, true);
        var listTip = await _collection.Find(filter).ToListAsync();
        return listTip;
    }
    public async Task<bool> DeleteTipAsync(string id, CancellationToken cancellationToken = default)
    {
        var filter = Builders<Tips>.Filter.Eq(o => o.Id, id);
        var restore = Builders<Tips>.Update.Set(r => r.Status, false);
        var restoreTip = await _collection.UpdateOneAsync(filter, restore);
        return restoreTip.IsAcknowledged && restoreTip.ModifiedCount < 0;
    }
    public async Task<bool> RestoreTipAsync(string id, CancellationToken cancellationToken = default)
    {
        var filter = Builders<Tips>.Filter.Eq(o => o.Id, id);
        var update = Builders<Tips>.Update.Set(u => u.Status, true);
        var deleteTip = await _collection.UpdateOneAsync(filter, update);
        return deleteTip.IsAcknowledged && deleteTip.ModifiedCount > 0;
    }

}
