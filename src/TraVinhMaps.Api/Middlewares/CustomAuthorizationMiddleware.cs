// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Net;
using System.Text.Json;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authorization.Policy;
using TraVinhMaps.Application.Common.Dtos;

namespace TraVinhMaps.Api.Middlewares;

public class CustomAuthorizationMiddleware : IAuthorizationMiddlewareResultHandler
{
    private readonly AuthorizationMiddlewareResultHandler _defaultHandler = new();

    public async Task HandleAsync(
        RequestDelegate next,
        HttpContext context,
        AuthorizationPolicy policy,
        PolicyAuthorizationResult authorizeResult)
    {
        // If the authorization was successful, continue with the next middleware
        if (authorizeResult.Succeeded)
        {
            await _defaultHandler.HandleAsync(next, context, policy, authorizeResult);
            return;
        }

        // If authorization failed, use our custom response format
        var statusCode = authorizeResult.Forbidden
            ? HttpStatusCode.Forbidden
            : HttpStatusCode.Unauthorized;

        var message = statusCode == HttpStatusCode.Forbidden
            ? "You do not have permission to access this resource"
            : "Authentication is required to access this resource";

        // Create a response using our custom format
        var response = ApiResponse.CreateError(message, statusCode);

        context.Response.StatusCode = (int)statusCode;
        context.Response.ContentType = "application/json; charset=utf-8";

        var options = new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        };

        await JsonSerializer.SerializeAsync(context.Response.Body, response, options);
    }
}
