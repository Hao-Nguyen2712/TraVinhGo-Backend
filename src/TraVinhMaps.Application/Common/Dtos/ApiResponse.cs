// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Net;

namespace TraVinhMaps.Application.Common.Dtos;

public class ApiResponse
{
    public string Message { get; set; } = string.Empty;
    public string Status { get; set; } = "success";
    public HttpStatusCode StatusCode { get; set; }
    public object? Errors { get; set; }

    public ApiResponse(string status, string message, HttpStatusCode statusCode)
    {
        Status = status;
        Message = message;
        StatusCode = statusCode;
    }

    public ApiResponse()
    {
        Status = "success";
        StatusCode = HttpStatusCode.OK;
    }

    public static ApiResponse Ok(string message = "")
    {
        return new ApiResponse("success", message, HttpStatusCode.OK);
    }

    public static ApiResponse CreateError(string message, HttpStatusCode statusCode = HttpStatusCode.BadRequest)
    {
        return new ApiResponse("error", message, statusCode);
    }

    public static ApiResponse CreateError(string message, IEnumerable<ApiError> errors, HttpStatusCode statusCode = HttpStatusCode.BadRequest)
    {
        var response = new ApiResponse("error", message, statusCode);
        response.Errors = errors;
        return response;
    }

    public ApiResponse AddError(string code, string detail, string field = "")
    {
        if (Errors == null)
        {
            Errors = new List<ApiError>();
        }

        if (Errors is List<ApiError> errorList)
        {
            errorList.Add(new ApiError(code, detail, field));
        }
        return this;
    }

    public ApiResponse AddErrors(IEnumerable<ApiError> errors)
    {
        if (Errors == null)
        {
            Errors = new List<ApiError>();
        }

        if (Errors is List<ApiError> errorList)
        {
            errorList.AddRange(errors);
        }
        return this;
    }
}
