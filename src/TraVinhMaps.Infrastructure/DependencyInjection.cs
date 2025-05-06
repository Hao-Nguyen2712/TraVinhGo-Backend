// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using Microsoft.Extensions.DependencyInjection;
using TraVinhMaps.Application.UnitOfWorks;
using TraVinhMaps.Infrastructure.Db;

namespace TraVinhMaps.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services)
    {
        services.AddHostedService<DbInitializationService>();
        services.AddScoped<IUnitOfWork, Infrastructure.UnitOfWork.UnitOfWork>();
        services.AddSingleton<IDbContext, DbContext>();
        services.AddSingleton(typeof(IRepository<>), typeof(Infrastructure.UnitOfWork.Repository<>));
        return services;
    }
}
