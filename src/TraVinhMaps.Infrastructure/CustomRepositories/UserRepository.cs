// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using MongoDB.Bson;
using MongoDB.Driver;
using TraVinhMaps.Application.UnitOfWorks;
using TraVinhMaps.Domain.Entities;
using TraVinhMaps.Domain.Specs;
using TraVinhMaps.Infrastructure.Db;
using TraVinhMaps.Infrastructure.UnitOfWork;

namespace TraVinhMaps.Infrastructure.CustomRepositories;
public class UserRepository : Repository<User>, IUserRepository
{
    public UserRepository(IDbContext context) : base(context)
    {
    }

    public async Task<bool> DeleteUser(string id, CancellationToken cancellationToken = default)
    {
        var filter = Builders<User>.Filter.Eq(u => u.Id, id);
        var update = Builders<User>.Update
            .Set(u => u.IsForbidden, true)
            .Set(u => u.Status, false)
            .Set(u => u.UpdatedAt, DateTime.UtcNow);

        var result = await _collection.UpdateOneAsync(filter, update, cancellationToken: cancellationToken);

        return result.ModifiedCount > 0;
    }

    public async Task<Pagination<User>> GetUserAsync(UserSpecParams userSpecParams, CancellationToken cancellationToken = default)
    {
        var builder = Builders<User>.Filter;
        var filter = builder.Eq(x => x.Status, true);

        // Search for: FirstName, LastName or Username
        if (!string.IsNullOrEmpty(userSpecParams.Search))
        {
            var regex = new BsonRegularExpression(userSpecParams.Search, "i");
            var searchFilter = builder.Or(
                builder.Regex(u => u.Profile.FirstName, regex),
                builder.Regex(u => u.Profile.LastName, regex),
                builder.Regex(u => u.Username, regex)
            );
            filter &= searchFilter;
        }

        // Total number of records that meet the condition
        var totalCount = await _collection.CountDocumentsAsync(filter, cancellationToken: cancellationToken);

        // Sort
        var sortBuilder = Builders<User>.Sort;
        SortDefinition<User> sortDefinition;

        switch (userSpecParams.Sort?.ToLower())
        {
            case "firstname_desc":
                sortDefinition = sortBuilder.Descending(x => x.Profile.FirstName);
                break;
            case "lastname_asc":
                sortDefinition = sortBuilder.Ascending(x => x.Profile.LastName);
                break;
            case "username_desc":
                sortDefinition = sortBuilder.Descending(x => x.Username);
                break;
            default:
                sortDefinition = sortBuilder.Ascending(x => x.Username); // Sort mặc định
                break;
        }

        // Lấy dữ liệu phân trang
        var users = await _collection.Find(filter)
            .Sort(sortDefinition)
            .Skip(userSpecParams.PageSize * (userSpecParams.PageIndex - 1))
            .Limit(userSpecParams.PageSize)
            .ToListAsync(cancellationToken);

        return new Pagination<User>
        {
            PageIndex = userSpecParams.PageIndex,
            PageSize = userSpecParams.PageSize,
            Count = totalCount,
            Data = users
        };
    }

    public async Task<bool> RestoreUser(string id, CancellationToken cancellationToken = default)
    {
        var filter = Builders<User>.Filter.Eq(u => u.Id, id);
        var update = Builders<User>.Update
            .Set(u => u.IsForbidden, false)
            .Set(u => u.Status, true)
            .Set(u => u.UpdatedAt, DateTime.UtcNow);

        var result = await _collection.UpdateOneAsync(filter, update, cancellationToken: cancellationToken);

        return result.ModifiedCount > 0;
    }
}
