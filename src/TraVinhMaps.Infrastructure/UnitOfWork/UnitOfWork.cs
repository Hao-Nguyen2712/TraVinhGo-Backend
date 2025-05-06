// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using MongoDB.Driver;
using TraVinhMaps.Application.UnitOfWorks;
using TraVinhMaps.Domain.Entities;
using TraVinhMaps.Infrastructure.Db;

namespace TraVinhMaps.Infrastructure.UnitOfWork;

public class UnitOfWork : IUnitOfWork
{
    private readonly IDbContext _context;
    private readonly Dictionary<Type, object> _repositories = new Dictionary<Type, object>();
    private bool _disposed;

    public UnitOfWork(IDbContext context)
    {
        _context = context;
    }
    public IMongoClient Client => _context.Client;

    public IMongoDatabase Database => _context.Database;

    public async Task AbortTransactionAsync(IClientSessionHandle session, CancellationToken cancellationToken = default)
    {
        if (session.IsInTransaction)
        {
            await session.AbortTransactionAsync(cancellationToken);
        }
    }

    public async Task<IClientSessionHandle> BeginTransactionAsync(CancellationToken cancellationToken = default)
    {
        var session = await _context.Client.StartSessionAsync(null, cancellationToken);
        session.StartTransaction();
        return session;
    }

    public async Task CommitTransactionAsync(IClientSessionHandle session, CancellationToken cancellationToken = default)
    {
        if (session.IsInTransaction)
        {
            await session.CommitTransactionAsync(cancellationToken);
        }
    }

    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    public IRepository<T> GetRepository<T>() where T : BaseEntity
    {
        if (_repositories.ContainsKey(typeof(T)))
        {
            return (IRepository<T>)_repositories[typeof(T)];
        }
        var repository = new Repository<T>(_context);
        _repositories.Add(typeof(T), repository);
        return repository;
    }

    protected virtual void Dispose(bool disposing)
    {
        if (!_disposed)
        {
            if (disposing)
            {
                // Dispose managed resources
                _repositories.Clear();
            }
            // Dispose unmanaged resources
            _disposed = true;
        }
    }
}
