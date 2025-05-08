// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Net;
using Microsoft.AspNetCore.Mvc;
using TraVinhMaps.Application.Common.Dtos;

namespace TraVinhMaps.Api.Extensions;

public static class ApiResponseExtensions
{
    public static ActionResult ApiResponse(this ControllerBase controller, ApiResponse response)
    {
        return controller.StatusCode((int)response.StatusCode, response);
    }
    public static ActionResult<T> ApiResponse<T>(this ControllerBase controller, ApiResponse<T> response)
    {
        return controller.StatusCode((int)response.StatusCode, response);
    }

    public static ActionResult Ok(this ControllerBase controller, string message = "")
    {
        return controller.ApiResponse(Application.Common.Dtos.ApiResponse.Ok(message));
    }

    public static ActionResult<T> Ok<T>(this ControllerBase controller, T data, string message = "")
    {
        return controller.ApiResponse(Application.Common.Dtos.ApiResponse<T>.Ok(data, message));
    }

    public static ActionResult Error(this ControllerBase controller, string message, HttpStatusCode statusCode = HttpStatusCode.BadRequest)
    {
        return controller.ApiResponse(Application.Common.Dtos.ApiResponse.Error(message, statusCode));
    }

    public static ActionResult Error(this ControllerBase controller, string message, IEnumerable<ApiError> errors, HttpStatusCode statusCode = HttpStatusCode.BadRequest)
    {
        return controller.ApiResponse(Application.Common.Dtos.ApiResponse.Error(message, errors, statusCode));
    }

    public static ActionResult<T> Error<T>(this ControllerBase controller, string message, HttpStatusCode statusCode = HttpStatusCode.BadRequest)
    {
        return controller.ApiResponse(Application.Common.Dtos.ApiResponse<T>.Error(message, statusCode));
    }

    public static ActionResult<T> Error<T>(this ControllerBase controller, string message, IEnumerable<ApiError> errors, HttpStatusCode statusCode = HttpStatusCode.BadRequest)
    {
        return controller.ApiResponse(Application.Common.Dtos.ApiResponse<T>.Error(message, errors, statusCode));
    }

    public static ActionResult NotFound(this ControllerBase controller, string message = "Resource not found")
    {
        return controller.ApiResponse(Application.Common.Dtos.ApiResponse.Error(message, HttpStatusCode.NotFound));
    }

    public static ActionResult NotFound(this ControllerBase controller, string message, IEnumerable<ApiError> errors)
    {
        return controller.ApiResponse(Application.Common.Dtos.ApiResponse.Error(message, errors, HttpStatusCode.NotFound));
    }

    public static ActionResult<T> NotFound<T>(this ControllerBase controller, string message = "Resource not found")
    {
        return controller.ApiResponse(Application.Common.Dtos.ApiResponse<T>.Error(message, HttpStatusCode.NotFound));
    }

    public static ActionResult<T> NotFound<T>(this ControllerBase controller, string message, IEnumerable<ApiError> errors)
    {
        return controller.ApiResponse(Application.Common.Dtos.ApiResponse<T>.Error(message, errors, HttpStatusCode.NotFound));
    }

    public static ActionResult Created<T>(this ControllerBase controller, T data, string message = "Resource created successfully")
    {
        var response = Application.Common.Dtos.ApiResponse<T>.Ok(data, message);
        response.StatusCode = HttpStatusCode.Created;
        return controller.StatusCode((int)response.StatusCode, response);
    }

}
