// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Net;

namespace TraVinhMaps.Application.Common.Dtos;

public class ApiResponse
{
    public bool IsSuccess { get; set; }
    public string Message { get; set; } = string.Empty;
    public HttpStatusCode StatusCode { get; set; }
    public List<ApiError> Errors { get; set; } = new List<ApiError>();

    public ApiResponse(bool isSuccess, string message, HttpStatusCode statusCode)
    {
        IsSuccess = isSuccess;
        Message = message;
        StatusCode = statusCode;
    }

    public ApiResponse()
    {
        IsSuccess = true;
        StatusCode = HttpStatusCode.OK;
    }
    public static ApiResponse Ok(string message = "")
    {
        return new ApiResponse(true, message, HttpStatusCode.OK);
    }

    public static ApiResponse Error(string message, HttpStatusCode statusCode = HttpStatusCode.BadRequest)
    {
        return new ApiResponse(false, message, statusCode);
    }

    public static ApiResponse Error(string message, IEnumerable<ApiError> errors, HttpStatusCode statusCode = HttpStatusCode.BadRequest)
    {
        var response = new ApiResponse(false, message, statusCode);
        response.Errors.AddRange(errors);
        return response;
    }

    public ApiResponse AddError(string code, string detail, string field = "")
    {
        Errors.Add(new ApiError(code, detail, field));
        return this;
    }

    public ApiResponse AddErrors(IEnumerable<ApiError> errors)
    {
        Errors.AddRange(errors);
        return this;
    }
}
