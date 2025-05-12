// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Net;
using System.Text.Json;
using TraVinhMaps.Application.Common.Dtos;

namespace TraVinhMaps.Api.Middlewares;

public class CustomAuthenticationMiddleware
{
    private readonly RequestDelegate _next;

    public CustomAuthenticationMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        // Save the original body stream
        var originalBodyStream = context.Response.Body;

        try
        {
            // Create a new memory stream for the response
            using var memoryStream = new MemoryStream();
            context.Response.Body = memoryStream;

            // Continue with the pipeline
            await _next(context);

            // If we get a 401 Unauthorized or 403 Forbidden from the original middleware
            if (context.Response.StatusCode == 401 || context.Response.StatusCode == 403)
            {
                // Reset the stream position
                memoryStream.SetLength(0);

                // Generate our custom response
                var statusCode = context.Response.StatusCode == 403
                    ? HttpStatusCode.Forbidden
                    : HttpStatusCode.Unauthorized;

                var message = statusCode == HttpStatusCode.Forbidden
                    ? "You do not have permission to access this resource"
                    : "Authentication is required to access this resource";

                // Create a response using our custom format
                var response = ApiResponse.CreateError(message, statusCode);

                // Ensure content type is set
                context.Response.ContentType = "application/json; charset=utf-8";

                var options = new JsonSerializerOptions
                {
                    PropertyNamingPolicy = JsonNamingPolicy.CamelCase
                };

                // Write directly to memory stream
                await JsonSerializer.SerializeAsync(memoryStream, response, options);
            }

            // Copy the memory stream to the original response stream
            memoryStream.Position = 0;
            await memoryStream.CopyToAsync(originalBodyStream);
        }
        finally
        {
            // Restore the original response body
            context.Response.Body = originalBodyStream;
        }
    }
}
