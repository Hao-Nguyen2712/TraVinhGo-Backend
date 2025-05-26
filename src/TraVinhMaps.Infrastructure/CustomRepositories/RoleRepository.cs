// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MongoDB.Bson;
using MongoDB.Driver;
using TraVinhMaps.Application.Features.Roles.Models;
using TraVinhMaps.Application.UnitOfWorks;
using TraVinhMaps.Domain.Entities;
using TraVinhMaps.Infrastructure.Db;
using TraVinhMaps.Infrastructure.UnitOfWork;

namespace TraVinhMaps.Infrastructure.CustomRepositories;
public class RoleRepository : Repository<Role>, IRoleRepository
{
    public RoleRepository(IDbContext context) : base(context)
    {
    }

    public async Task<Role> AddAsync(RoleRequest entity, CancellationToken cancellationToken)
    {
        if (string.IsNullOrEmpty(entity.RoleName))
        {
            throw new ArgumentException("RoleName is required.");
        }
        // Check if email already exists
        var existingRole = await _collection.Find(x => x.RoleName == entity.RoleName).FirstOrDefaultAsync(cancellationToken);
        if (existingRole != null)
        {
            throw new ArgumentException("Role already exists.");
        }

        var role = new Role
        {
            Id = ObjectId.GenerateNewId().ToString(),
            RoleName = entity.RoleName,
            RoleStatus = true,
            CreatedAt = DateTime.UtcNow,
        };
        await AddAsync(role, cancellationToken);
        return role;
    }

    public async Task<bool> DeleteAsync(string id, CancellationToken cancellationToken = default)
    {
        var filter = Builders<Role>.Filter.Eq(u => u.Id, id);
        var update = Builders<Role>.Update
            .Set(u => u.RoleStatus, false);

        var result = await _collection.UpdateOneAsync(filter, update, cancellationToken: cancellationToken);

        return result.ModifiedCount > 0;
    }

    public async Task<bool> UpdateAsync(string id, RoleRequest entity, CancellationToken cancellationToken = default)
    {
        var filter = Builders<Role>.Filter.Eq(u => u.Id, id);
        var update = Builders<Role>.Update
            .Set(u => u.RoleName, entity.RoleName)
            .Set(u => u.RoleStatus, true);

        var result = await _collection.UpdateOneAsync(filter, update, cancellationToken: cancellationToken);

        return result.ModifiedCount > 0;
    }

}
