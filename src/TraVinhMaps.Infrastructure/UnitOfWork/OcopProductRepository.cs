// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MongoDB.Bson;
using MongoDB.Driver;
using TraVinhMaps.Application.UnitOfWorks;
using TraVinhMaps.Domain.Entities;
using TraVinhMaps.Domain.Specs;
using TraVinhMaps.Infrastructure.Db;

namespace TraVinhMaps.Infrastructure.UnitOfWork;
public class OcopProductRepository : Repository<OcopProduct>, IOcopProductRepository
{
    public OcopProductRepository(IDbContext dbContext) : base(dbContext) { }
    public async Task<Pagination<OcopProduct>> GetAllOcopProductAsync(OcopProductSpecParams ocopProductSpecParams)
    {
        var builder = Builders<OcopProduct>.Filter;
        var filter = builder.Eq(o => o.Status, true);
        if (!string.IsNullOrEmpty(ocopProductSpecParams.Search))
        {
            var searchFilter = builder.Regex(x => x.ProductName, new BsonRegularExpression(ocopProductSpecParams.Search));
            filter &= searchFilter;
        }
        if (!string.IsNullOrEmpty(ocopProductSpecParams.Sort))
        {
            return new Pagination<OcopProduct>
            {
                PageSize = ocopProductSpecParams.PageSize,
                PageIndex = ocopProductSpecParams.PageIndex,
                Data = await DataFilter(ocopProductSpecParams, filter),
                Count = await _collection.CountDocumentsAsync(filter)
            };
        }
        if (!string.IsNullOrEmpty(ocopProductSpecParams.TypeId))
        {
            var typeFilter = builder.Eq(o => o.Id, ocopProductSpecParams.TypeId);
            filter &= typeFilter;
        }
        return new Pagination<OcopProduct>
        {
            PageSize = ocopProductSpecParams.PageSize,
            PageIndex = ocopProductSpecParams.PageIndex,
            Data = await _collection
                    .Find(filter)
                    .Sort(Builders<OcopProduct>.Sort.Ascending("ProductName"))
                    .Skip(ocopProductSpecParams.PageSize * (ocopProductSpecParams.PageIndex - 1))
                    .Limit(ocopProductSpecParams.PageSize)
                    .ToListAsync(),
            Count = await _collection.CountDocumentsAsync(filter)
        };
    }
    private async Task<IReadOnlyList<OcopProduct>> DataFilter(OcopProductSpecParams ocopProductSpecParams, FilterDefinition<OcopProduct> filter)
    {
        switch (ocopProductSpecParams.Sort)
        {
            case "nameAsc":
                return await _collection
                    .Find(filter)
                    .Sort(Builders<OcopProduct>.Sort.Ascending("ProductName"))
                    .Skip(ocopProductSpecParams.PageSize * (ocopProductSpecParams.PageIndex - 1))
                    .Limit(ocopProductSpecParams.PageSize)
                    .ToListAsync();
            case "nameDesc":
                return await _collection
                    .Find(filter)
                    .Sort(Builders<OcopProduct>.Sort.Descending("ProductName"))
                    .Skip(ocopProductSpecParams.PageSize * (ocopProductSpecParams.PageIndex - 1))
                    .Limit(ocopProductSpecParams.PageSize)
                    .ToListAsync();
            default:
                return await _collection
                    .Find(filter)
                    .Sort(Builders<OcopProduct>.Sort.Ascending("ProductName"))
                    .Skip(ocopProductSpecParams.PageSize * (ocopProductSpecParams.PageIndex - 1))
                    .Limit(ocopProductSpecParams.PageSize)
                    .ToListAsync();
        }
    }
    public async Task<IEnumerable<OcopProduct>> GetAllOcopProductActiveAsync(CancellationToken cancellationToken = default)
    {
        var filter = Builders<OcopProduct>.Filter.Eq(o => o.Status, true);
        var listOcopProduct = await _collection.Find(filter).ToListAsync();
        return listOcopProduct;
    }

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

    public async Task<String> AddImageOcopProduct(string id, string imageUrl, CancellationToken cancellationToken = default)
    {
        var filter = Builders<OcopProduct>.Filter.Eq(o => o.Id, id);
        var ocopProduct = await _collection.Find(filter).FirstOrDefaultAsync(cancellationToken);
        if (ocopProduct == null) return null;
        if (ocopProduct.ProductImage == null)
        {
            var newListImage = Builders<OcopProduct>.Update.Set(im => im.ProductImage, new List<String>());
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
}
