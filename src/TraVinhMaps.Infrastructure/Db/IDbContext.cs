// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using MongoDB.Driver;

namespace TraVinhMaps.Infrastructure.Db;

public interface IDbContext
{
    IMongoDatabase Database { get; }
    IMongoClient Client { get; }
    IMongoCollection<T> GetCollection<T>();
    Task InitializeDatabaseAsync();
}
