// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using Microsoft.Extensions.DependencyInjection;
using TraVinhMaps.Application.External;
using TraVinhMaps.Application.Features.Admins;
using TraVinhMaps.Application.Features.Admins.Interface;
using TraVinhMaps.Application.Features.Auth;
using TraVinhMaps.Application.Features.Auth.Interface;
using TraVinhMaps.Application.Features.CommunityTips;
using TraVinhMaps.Application.Features.CommunityTips.Interface;
using TraVinhMaps.Application.Features.Destination;
using TraVinhMaps.Application.Features.Destination.Interface;
using TraVinhMaps.Application.Features.EventAndFestivalFeature;
using TraVinhMaps.Application.Features.EventAndFestivalFeature.Interface;
using TraVinhMaps.Application.Features.Notifications;
using TraVinhMaps.Application.Features.Notifications.Interface;
using TraVinhMaps.Application.Features.OcopProduct;
using TraVinhMaps.Application.Features.OcopProduct.Interface;
using TraVinhMaps.Application.Features.Roles;
using TraVinhMaps.Application.Features.Roles.Interface;
using TraVinhMaps.Application.Features.Tags;
using TraVinhMaps.Application.Features.Tags.Interface;
using TraVinhMaps.Application.Features.Users;
using TraVinhMaps.Application.Features.Users.Interface;
using TraVinhMaps.Application.UnitOfWorks;
using TraVinhMaps.Infrastructure.CustomRepositories;
using TraVinhMaps.Infrastructure.Db;
using TraVinhMaps.Infrastructure.External;
using TraVinhMaps.Infrastructure.UnitOfWork;

namespace TraVinhMaps.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services)
    {
        services.AddHostedService<DbInitializationService>();
        services.AddSingleton<IDbContext, DbContext>();
        services.AddSingleton(typeof(IRepository<>), typeof(Infrastructure.UnitOfWork.Repository<>));
        services.AddScoped<IAuthServices, AuthService>();
        // Cloudinary
        services.AddScoped<ICloudinaryService, CloudinaryService>();
        // SpeedSmsApi
        services.AddSingleton<ISpeedSmsService, SpeedSmsService>();
        // Redis
        services.AddSingleton<ICacheService, CacheService>();
        //mail service
        services.AddTransient<IEmailSender, EmailSender>();
        //TouristDestination
        services.AddScoped<ITouristDestinationRepository, TouristDestinationRepository>();
        services.AddScoped<ITouristDestinationService, TouristDestinationService>();
        services.AddScoped<ImageManagementDestinationServices>();
        //EventAndFestival
        services.AddScoped<IEventAndFestivalRepository, EventAndFestivalRepository>();
        services.AddScoped<IEventAndFestivalService, EventAndFestivalService>();
        services.AddScoped<ImageManagementEventAndFestivalServices>();

        // User
        services.AddScoped<IUserRepository, UserRepository>();
        services.AddScoped<IUserService, UserService>();
        services.AddScoped<UploadImageUser>();

        // OcopProduct
        services.AddScoped<IOcopProductRepository, OcopProductRepository>();
        services.AddScoped<IOcopProductService, OcopProductService>();
        services.AddScoped<ImageManagementOcopProductServices>();

        //CommunityTips
        services.AddScoped<ICommunityTipsRepository, CommunityTipsRepository>();
        services.AddScoped<ICommunityTipsService, CommunityTipsService>();

        // FluentValidation
        // services.AddScoped<IValidator<User>, UserValidator>();

        // Notification
        services.AddScoped<INotificationsRepository, NotificationsRepository>();
        services.AddScoped<INotificationService, NotificationsService>();

        // Admin Management
        services.AddScoped<IAdminRepository, AdminsRepository>();
        services.AddScoped<IAdminService, AdminService>();

        // Role Management
        services.AddScoped<IRoleRepository, RoleRepository>();
        services.AddScoped<IRoleService, RoleService>();

        // Tags
        services.AddScoped<ITagService, TagService>();

        services.AddHttpClient();

        return services;
    }
}
