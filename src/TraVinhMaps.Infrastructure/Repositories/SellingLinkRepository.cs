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
public class SellingLinkRepository : BaseRepository<SellingLink>, ISellingLinkRepository
{
    public SellingLinkRepository(IDbContext dbContext) : base(dbContext) { }
    public async Task<IEnumerable<SellingLink>> GetSellingLinkByProductId(string productId, CancellationToken cancellationToken = default)
    {
        var filter = Builders<SellingLink>.Filter.Eq(c => c.ProductId, productId);
        var sellingLinks= await _collection.Find(filter).ToListAsync();
        return sellingLinks;
    }
}
