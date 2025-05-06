// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Linq.Expressions;
using MongoDB.Driver;
using TraVinhMaps.Application.UnitOfWorks;
using TraVinhMaps.Domain.Entities;
using TraVinhMaps.Infrastructure.Db;

namespace TraVinhMaps.Infrastructure.UnitOfWork;

public class Repository<T> : IRepository<T> where T : BaseEntity
{
    private readonly IDbContext _context;
    private readonly IMongoCollection<T> _collection;

    public Repository(IDbContext context)
    {
        _context = context;
        _collection = _context.GetCollection<T>();
    }

    public async Task<T> AddAsync(T entity, CancellationToken cancellationToken = default)
    {
        await _collection.InsertOneAsync(entity, null, cancellationToken);
        return entity;
    }

    public async Task<IEnumerable<T>> AddRangeAsync(IEnumerable<T> entities, CancellationToken cancellationToken = default)
    {
        await _collection.InsertManyAsync(entities, null, cancellationToken);
        return entities;
    }

    public async Task<long> CountAsync(Expression<Func<T, bool>> predicate = null, CancellationToken cancellationToken = default)
    {
        return predicate == null
            ? await _collection.CountDocumentsAsync(FilterDefinition<T>.Empty, null, cancellationToken)
            : await _collection.CountDocumentsAsync(predicate, null, cancellationToken);
    }

    public async Task DeleteAsync(T entity, CancellationToken cancellationToken = default)
    {
        var filter = Builders<T>.Filter.Eq(e => e.Id, entity.Id);
        await _collection.DeleteOneAsync(filter, cancellationToken);
    }

    public async Task<T> GetByIdAsync(string id, CancellationToken cancellationToken = default)
    {
        var filter = Builders<T>.Filter.Eq(e => e.Id, id);
        return await _collection.Find(filter).FirstOrDefaultAsync(cancellationToken);
    }

    public async Task<IEnumerable<T>> ListAllAsync(CancellationToken cancellationToken = default)
    {
        return await _collection.Find(FilterDefinition<T>.Empty).ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<T>> ListAsync(Expression<Func<T, bool>> predicate, CancellationToken cancellationToken = default)
    {
        return await _collection.Find(predicate).ToListAsync(cancellationToken);
    }

    public async Task UpdateAsync(T entity, CancellationToken cancellationToken = default)
    {
        var filter = Builders<T>.Filter.Eq(e => e.Id, entity.Id);
        await _collection.ReplaceOneAsync(filter, entity, new ReplaceOptions { IsUpsert = false }, cancellationToken);
    }
}
