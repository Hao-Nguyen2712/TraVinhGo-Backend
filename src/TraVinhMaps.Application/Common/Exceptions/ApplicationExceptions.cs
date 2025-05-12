// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Net;
using TraVinhMaps.Application.Common.Dtos;

namespace TraVinhMaps.Application.Common.Exceptions;

public class ApplicationException : Exception
{
    public HttpStatusCode StatusCode { get; }

    public ApplicationException(string message, HttpStatusCode statusCode = HttpStatusCode.BadRequest)
        : base(message)
    {
        StatusCode = statusCode;
    }
}

public class BadRequestException : ApplicationException
{
    public IEnumerable<ApiError> Errors { get; }

    public BadRequestException(string message)
        : base(message, HttpStatusCode.BadRequest)
    {
        Errors = Array.Empty<ApiError>();
    }

    public BadRequestException(string message, IEnumerable<ApiError> errors)
        : base(message, HttpStatusCode.BadRequest)
    {
        Errors = errors;
    }
}

public class NotFoundException : ApplicationException
{
    public NotFoundException(string message)
        : base(message, HttpStatusCode.NotFound)
    {
    }

    public NotFoundException(string name, object key)
        : base($"Entity \"{name}\" ({key}) was not found.", HttpStatusCode.NotFound)
    {
    }
}

public class UnauthorizedException : ApplicationException
{
    public UnauthorizedException(string message)
        : base(message, HttpStatusCode.Unauthorized)
    {
    }
}

public class ForbiddenException : ApplicationException
{
    public ForbiddenException(string message)
        : base(message, HttpStatusCode.Forbidden)
    {
    }
}
