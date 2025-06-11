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
public class TagRepository : BaseRepository<Tags>, ITagRepository
{
    public TagRepository(IDbContext context) : base(context)
    {
    }

    public async Task<string> GetTagIdByNameAsync(string tagName, CancellationToken cancellationToken = default)
    {
        var filter = Builders<Tags>.Filter.Eq(t => t.Name, tagName);
        var tags = await _collection.Find(filter).FirstOrDefaultAsync(cancellationToken);
        return tags.Id;
    }
}
