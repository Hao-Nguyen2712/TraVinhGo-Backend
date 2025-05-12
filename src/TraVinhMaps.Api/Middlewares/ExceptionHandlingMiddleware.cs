// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Net;
using System.Text.Json;
using TraVinhMaps.Application.Common.Dtos;
using TraVinhMaps.Application.Common.Exceptions;

namespace TraVinhMaps.Api.Middlewares;

public class ExceptionHandlingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<ExceptionHandlingMiddleware> _logger;

    public ExceptionHandlingMiddleware(RequestDelegate next, ILogger<ExceptionHandlingMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An unhandled exception occurred");
            await HandleExceptionAsync(context, ex);
        }
    }

    private static async Task HandleExceptionAsync(HttpContext context, Exception exception)
    {
        context.Response.ContentType = "application/json";

        var response = exception switch
        {
            // Add handling for custom exceptions here
            BadRequestException ex => CreateResponse(ex.Message, HttpStatusCode.BadRequest, ex.Errors),
            NotFoundException ex => CreateResponse(ex.Message, HttpStatusCode.NotFound),
            UnauthorizedException ex => CreateResponse(ex.Message, HttpStatusCode.Unauthorized),
            // Default case for unhandled exceptions
            _ => CreateResponse("An unexpected error occurred", HttpStatusCode.InternalServerError)
        };

        context.Response.StatusCode = (int)response.StatusCode;

        var options = new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        };

        var json = JsonSerializer.Serialize(response, options);
        await context.Response.WriteAsync(json);
    }

    private static ApiResponse CreateResponse(string message, HttpStatusCode statusCode, IEnumerable<ApiError>? errors = null)
    {
        var response = ApiResponse.CreateError(message, statusCode);

        if (errors != null)
        {
            response.Errors = errors;
        }

        return response;
    }
}
