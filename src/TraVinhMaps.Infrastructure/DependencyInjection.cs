// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using Microsoft.Extensions.DependencyInjection;
using TraVinhMaps.Application.External;
using TraVinhMaps.Application.Features.Destination.Interface;
using TraVinhMaps.Application.Features.Destination;
using TraVinhMaps.Application.UnitOfWorks;
using TraVinhMaps.Infrastructure.Db;
using TraVinhMaps.Infrastructure.External;
using TraVinhMaps.Infrastructure.UnitOfWork;
using TraVinhMaps.Application.Features.Users;
using TraVinhMaps.Infrastructure.CustomRepositories;
using TraVinhMaps.Application.Features.Users.Interface;
using FluentValidation;
using TraVinhMaps.Domain.Entities;
using TraVinhMaps.Application.Validators;

namespace TraVinhMaps.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services)
    {
        services.AddHostedService<DbInitializationService>();
        services.AddScoped<IUnitOfWork, Infrastructure.UnitOfWork.UnitOfWork>();
        services.AddSingleton<IDbContext, DbContext>();
        services.AddSingleton(typeof(IRepository<>), typeof(Infrastructure.UnitOfWork.Repository<>));

        // Cloudinary
        services.AddScoped<ICloudinaryService, CloudinaryService>();

        //TouristDestination
        services.AddScoped<ITouristDestinationRepository, TouristDestinationRepository>();
        services.AddScoped<ITouristDestinationService, TouristDestinationService>();
        services.AddScoped<ImageManagementDestinationServices>();

        // User
        services.AddScoped<IUserRepository, UserRepository>();
        services.AddScoped<IUserService, UserService>();
        services.AddScoped<UploadImageUser>();

        // FluentValidation
        // services.AddScoped<IValidator<User>, UserValidator>();

        return services;
    }
}
