// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using MongoDB.Driver;
using TraVinhMaps.Domain.Entities;

namespace TraVinhMaps.Application.UnitOfWorks;

public interface IUnitOfWork : IDisposable
{
    public IMongoClient Client { get; }
    public IMongoDatabase Database { get; }
    public IRepository<T> GetRepository<T>() where T : BaseEntity;
    public Task<IClientSessionHandle> BeginTransactionAsync(CancellationToken cancellationToken = default);
    public Task CommitTransactionAsync(IClientSessionHandle session, CancellationToken cancellationToken = default);
    public Task AbortTransactionAsync(IClientSessionHandle session, CancellationToken cancellationToken = default);
}
