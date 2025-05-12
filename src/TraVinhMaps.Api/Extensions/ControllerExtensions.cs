// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Net;
using Microsoft.AspNetCore.Mvc;
using TraVinhMaps.Application.Common.Dtos;

namespace TraVinhMaps.Api.Extensions;

public static class ControllerExtensions
{
    public static IActionResult ApiResult(this ControllerBase controller, ApiResponse response)
    {
        return controller.StatusCode((int)response.StatusCode, response);
    }

    public static IActionResult ApiOk(this ControllerBase controller, string message = "")
    {
        var response = ApiResponse.Ok(message);
        return controller.StatusCode((int)response.StatusCode, response);
    }

    public static IActionResult ApiOk<T>(this ControllerBase controller, T data, string message = "")
    {
        var response = ApiResponse<T>.Ok(data, message);
        return controller.StatusCode((int)response.StatusCode, response);
    }

    public static IActionResult ApiError(this ControllerBase controller, string message, HttpStatusCode statusCode = HttpStatusCode.BadRequest)
    {
        var response = ApiResponse.CreateError(message, statusCode);
        return controller.StatusCode((int)response.StatusCode, response);
    }

    public static IActionResult ApiError(this ControllerBase controller, string message, IEnumerable<ApiError> errors, HttpStatusCode statusCode = HttpStatusCode.BadRequest)
    {
        var response = ApiResponse.CreateError(message, errors, statusCode);
        return controller.StatusCode((int)response.StatusCode, response);
    }
}
