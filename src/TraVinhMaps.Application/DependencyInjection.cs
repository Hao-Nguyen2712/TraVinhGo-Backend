// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using Microsoft.Extensions.DependencyInjection;
using TraVinhMaps.Application.Features.Auth;
using TraVinhMaps.Application.Features.Auth.Interface;

namespace TraVinhMaps.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        // usecase Di
        services.AddScoped<IAuthServices, AuthService>();

        return services;
    }
}
