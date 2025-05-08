// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Net;

namespace TraVinhMaps.Application.Common.Dtos;

public class ApiResponse<T> : ApiResponse
{
    public T? Data { get; set; }
    public ApiResponse() : base()
    {
    }

    public ApiResponse(bool isSuccess, string message, T? data, HttpStatusCode statusCode)
       : base(isSuccess, message, statusCode)
    {
        Data = data;
    }
    public static ApiResponse<T> Ok(T data, string message = "")
    {
        return new ApiResponse<T>(true, message, data, HttpStatusCode.OK);
    }

    public static new ApiResponse<T> Error(string message, HttpStatusCode statusCode = HttpStatusCode.BadRequest)
    {
        return new ApiResponse<T>(false, message, default, statusCode);
    }

    public static ApiResponse<T> Error(string message, IEnumerable<ApiError> errors, HttpStatusCode statusCode = HttpStatusCode.BadRequest)
    {
        var response = new ApiResponse<T>(false, message, default, statusCode);
        response.AddErrors(errors);
        return response;
    }

    public new ApiResponse<T> AddError(string code, string detail, string field = "")
    {
        base.AddError(code, detail, field);
        return this;
    }

    public new ApiResponse<T> AddErrors(IEnumerable<ApiError> errors)
    {
        base.AddErrors(errors);
        return this;
    }
}

