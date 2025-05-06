// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using Microsoft.Extensions.Configuration;
using MongoDB.Driver;
using TraVinhMaps.Domain.Entities;

namespace TraVinhMaps.Infrastructure.Db;

public class DbContext : IDbContext
{
    private readonly IMongoDatabase _database;
    private readonly IMongoClient _client;

    public DbContext(IConfiguration configuration)
    {
        var connectionString = configuration["MongoDb:ConnectionString"];
        var databaseName = configuration["MongoDb:DatabaseName"];

        _client = new MongoClient(connectionString);
        _database = _client.GetDatabase(databaseName);
    }

    public IMongoDatabase Database => _database;

    public IMongoClient Client => _client;

    public IMongoCollection<T> GetCollection<T>()
    {
        return _database.GetCollection<T>(typeof(T).Name);
    }

    public async Task InitializeDatabaseAsync()
    {
        // Get the assembly containing the BaseEntity class
        var domainAssembly = typeof(BaseEntity).Assembly;

        // Find all non-abstract classes that inherit from BaseEntity
        var entityTypes = domainAssembly.GetTypes()
             .Where(t => t.IsClass && !t.IsAbstract &&
                        t.IsSubclassOf(typeof(BaseEntity)))
             .ToList();

        // Create a collection for each entity type if it doesn't exist
        foreach (var entityType in entityTypes)
        {
            await EnsureCollectionCreatedAsync(entityType);
        }
    }

    private async Task EnsureCollectionCreatedAsync(Type entityType)
    {
        var collectionName = entityType.Name;

        // Get a list of all collection names in the database
        var collectionNames = await (await _database.ListCollectionNamesAsync()).ToListAsync();

        // Check if the collection exists
        if (!collectionNames.Contains(collectionName))
        {
            await _database.CreateCollectionAsync(collectionName);
        }
    }
}
