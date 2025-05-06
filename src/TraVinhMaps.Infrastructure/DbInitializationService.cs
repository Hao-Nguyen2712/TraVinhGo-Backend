// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using Microsoft.Extensions.Hosting;
using TraVinhMaps.Infrastructure.Db;

namespace TraVinhMaps.Infrastructure;

public class DbInitializationService : IHostedService
{
    private readonly IDbContext _context;

    public DbInitializationService(IDbContext context)
    {
        _context = context;
    }

    public async Task StartAsync(CancellationToken cancellationToken)
    {
        await _context.InitializeDatabaseAsync();
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        return Task.CompletedTask;
    }
}
