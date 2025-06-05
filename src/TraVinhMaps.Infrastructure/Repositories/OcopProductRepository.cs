// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using MongoDB.Driver;
using TraVinhMaps.Application.UnitOfWorks;
using TraVinhMaps.Domain.Entities;
using TraVinhMaps.Infrastructure.Db;

namespace TraVinhMaps.Infrastructure.CustomRepositories;
public class OcopProductRepository : BaseRepository<OcopProduct>, IOcopProductRepository
{
    public OcopProductRepository(IDbContext dbContext) : base(dbContext) { }
    public async Task<IEnumerable<OcopProduct>> GetOcopProductByCompanyId(string companyId, CancellationToken cancellationToken = default)
    {
        var filter = Builders<OcopProduct>.Filter.Eq(c => c.CompanyId, companyId) & Builders<OcopProduct>.Filter.Eq(s => s.Status, true);
        var ocopProduct = await _collection.Find(filter).ToListAsync();
        return ocopProduct;
    }

    public async Task<IEnumerable<OcopProduct>> GetOcopProductByOcopTypeId(string ocopTypeId, CancellationToken cancellationToken = default)
    {
        var filter = Builders<OcopProduct>.Filter.Eq(c => c.OcopTypeId, ocopTypeId) & Builders<OcopProduct>.Filter.Eq(s => s.Status, true);
        var ocopProduct = await _collection.Find(filter).ToListAsync();
        return ocopProduct;
    }
    public async Task<bool> DeleteOcopProductAsync(string id, CancellationToken cancellationToken = default)
    {
        var filter = Builders<OcopProduct>.Filter.Eq(o => o.Id, id);
        var restore = Builders<OcopProduct>.Update.Set(r => r.Status, false);
        var restoreOcopProduct = await _collection.UpdateOneAsync(filter, restore);
        return restoreOcopProduct.IsAcknowledged && restoreOcopProduct.ModifiedCount < 0;
    }
    public async Task<bool> RestoreOcopProductAsync(string id, CancellationToken cancellationToken = default)
    {
        var filter = Builders<OcopProduct>.Filter.Eq(o => o.Id, id);
        var restore = Builders<OcopProduct>.Update.Set(r => r.Status, true);
        var restoreOcopProduct = await _collection.UpdateOneAsync(filter, restore);
        return restoreOcopProduct.IsAcknowledged && restoreOcopProduct.ModifiedCount < 0;
    }

    public async Task<string> AddImageOcopProduct(string id, string imageUrl, CancellationToken cancellationToken = default)
    {
        var filter = Builders<OcopProduct>.Filter.Eq(o => o.Id, id);
        var ocopProduct = await _collection.Find(filter).FirstOrDefaultAsync(cancellationToken);
        if (ocopProduct == null) return null;
        if (ocopProduct.ProductImage == null)
        {
            var newListImage = Builders<OcopProduct>.Update.Set(im => im.ProductImage, new List<string>());
            await _collection.UpdateOneAsync(filter, newListImage);
        }
        var updateImage = Builders<OcopProduct>.Update.Push(p => p.ProductImage, imageUrl);
        var updateResult = await _collection.UpdateOneAsync(filter, updateImage, cancellationToken: cancellationToken);
        return imageUrl;
    }
    public async Task<SellLocation> AddSellLocation(string id, SellLocation sellLocation, CancellationToken cancellationToken = default)
    {
        var filter = Builders<OcopProduct>.Filter.Eq(o => o.Id, id);
        var ocopProduct = await _collection.Find(filter).FirstOrDefaultAsync(cancellationToken);
        if (ocopProduct == null) return null;
        if (ocopProduct.Sellocations == null)
        {
            var newListSellLocation = Builders<OcopProduct>.Update.Set(im => im.Sellocations, new List<SellLocation>());
            await _collection.UpdateOneAsync(filter, newListSellLocation);
        }
        var updateSellLocaton = Builders<OcopProduct>.Update.Push(p => p.Sellocations, sellLocation);
        var updateResult = await _collection.UpdateOneAsync(filter, updateSellLocaton, cancellationToken: cancellationToken);
        return sellLocation;
    }

    public async Task<bool> DeleteSellLocation(string ocopProductId, string sellLocationName, CancellationToken cancellationToken = default)
    {
        var filter = Builders<OcopProduct>.Filter.And(Builders<OcopProduct>.Filter.Eq(o => o.Id, ocopProductId), Builders<OcopProduct>.Filter.ElemMatch(p => p.Sellocations, s => s.LocationName == sellLocationName));
        var update = Builders<OcopProduct>.Update.PullFilter(p => p.Sellocations, s => s.LocationName == sellLocationName);
        var result = await _collection.UpdateOneAsync(filter, update, cancellationToken: cancellationToken);
        return result.IsAcknowledged && result.ModifiedCount > 0;
    }

    public async Task<bool> UpdateSellLocation(string ocopProductId, SellLocation sellLocation, CancellationToken cancellationToken = default)
    {
        var filter = Builders<OcopProduct>.Filter.Eq(o => o.Id, ocopProductId);
        var ocopProduct = await _collection.Find(filter).FirstOrDefaultAsync(cancellationToken);
        if (ocopProduct == null || ocopProduct.Sellocations == null) return false;
        var index = ocopProduct.Sellocations.FindIndex(s => s.LocationName == sellLocation.LocationName);
        ocopProduct.Sellocations[index] = sellLocation;
        var update = Builders<OcopProduct>.Update.Set(o => o.Sellocations, ocopProduct.Sellocations);
        var result = await _collection.UpdateOneAsync(filter, update, cancellationToken: cancellationToken);
        return result.IsAcknowledged && result.ModifiedCount > 0;
    }
}
