// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using MongoDB.Bson;
using MongoDB.Driver;
using TraVinhMaps.Application.Common.Extensions;
using TraVinhMaps.Application.Features.Admins.Mappers;
using TraVinhMaps.Application.Features.Admins.Models;
using TraVinhMaps.Application.UnitOfWorks;
using TraVinhMaps.Domain.Entities;
using TraVinhMaps.Infrastructure.Db;
using TraVinhMaps.Infrastructure.UnitOfWork;

namespace TraVinhMaps.Infrastructure.CustomRepositories;
public class AdminsRepository : Repository<User>, IAdminRepository
{
    public AdminsRepository(IDbContext context) : base(context)
    {
    }

    public async Task<User> AddAsync(AdminRequest entity, CancellationToken cancellationToken)
    {
        if (string.IsNullOrEmpty(entity.Email) || string.IsNullOrEmpty(entity.Password) || string.IsNullOrEmpty(entity.RoleId))
        {
            throw new ArgumentException("Username, Password, and RoleId are required.");
        }
        // Check if email already exists
        var existingAdmin = await _collection.Find(x => x.Email == entity.Email).FirstOrDefaultAsync(cancellationToken);
        if (existingAdmin != null)
        {
            throw new ArgumentException("Email already exists.");
        }

        // Check if phone number already exists (if provided)
        //if (!string.IsNullOrEmpty(entity.PhoneNumber))
        //{
        //    var existingPhoneUser = await _collection.Find(x => x.PhoneNumber == entity.PhoneNumber).FirstOrDefaultAsync(cancellationToken);
        //    if (existingPhoneUser != null)
        //    {
        //        throw new ArgumentException("Phone number already exists.");
        //    }
        //}

        var admin = new User
        {
            Id = ObjectId.GenerateNewId().ToString(),
            Username = entity.Username,
            Password = HashingTokenExtension.HashToken(entity.Password),
            RoleId = entity.RoleId,
            CreatedAt = DateTime.UtcNow,
            Status = true,
            IsForbidden = false,
            Email = entity.Email,
            PhoneNumber = entity.PhoneNumber,
            Profile = null,
            Favorites = null,
            UpdatedAt = null
        };
        await AddAsync(admin, cancellationToken);
        return admin;
    }

    public async Task<bool> DeleteAdmin(string id, CancellationToken cancellationToken = default)
    {
        var filter = Builders<User>.Filter.Eq(u => u.Id, id);
        var update = Builders<User>.Update
            .Set(u => u.IsForbidden, true)
            .Set(u => u.Status, false)
            .Set(u => u.UpdatedAt, DateTime.UtcNow);

        var result = await _collection.UpdateOneAsync(filter, update, cancellationToken: cancellationToken);

        return result.ModifiedCount > 0;
    }

    public async Task<bool> RestoreAdmin(string id, CancellationToken cancellationToken = default)
    {
        var filter = Builders<User>.Filter.Eq(u => u.Id, id);
        var update = Builders<User>.Update
            .Set(u => u.IsForbidden, false)
            .Set(u => u.Status, true)
            .Set(u => u.UpdatedAt, DateTime.UtcNow);

        var result = await _collection.UpdateOneAsync(filter, update, cancellationToken: cancellationToken);

        return result.ModifiedCount > 0;
    }

    public async Task<User> UpdateAsync(UpdateAdminRequest entity, CancellationToken cancellationToken = default)
    {
        var existingAdmin = await _collection.Find(x => x.Id == entity.Id).FirstOrDefaultAsync(cancellationToken);
        if (existingAdmin == null)
        {
            throw new ArgumentException("Admin not found.");
        }

        // update info
        var updateAdmin = AdminMapper.Mapper.Map<UpdateAdminRequest, User>(entity);
        updateAdmin.Id = existingAdmin.Id;
        updateAdmin.Email = existingAdmin.Email;
        updateAdmin.RoleId = existingAdmin.RoleId;
        updateAdmin.Status = existingAdmin.Status;
        updateAdmin.IsForbidden = existingAdmin.IsForbidden;
        updateAdmin.Password = string.IsNullOrEmpty(entity.Password)
        ? existingAdmin.Password
        : HashingTokenExtension.HashToken(entity.Password);

        existingAdmin.CreatedAt = existingAdmin.CreatedAt;
        existingAdmin.UpdatedAt = DateTime.UtcNow;


        // ReplaceOne
        var filter = Builders<User>.Filter.Eq(u => u.Id, existingAdmin.Id);
        await _collection.ReplaceOneAsync(filter, updateAdmin, cancellationToken: cancellationToken);

        return updateAdmin;
    }
}
