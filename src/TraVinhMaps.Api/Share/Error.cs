// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

namespace TraVinhMaps.Api.Share;

public class Error
{
    public Error(string message)
    {
        Message = message;
    }

    public string Message { get; }

    public static Error None => new(string.Empty);

    public static implicit operator Error(string message) => new(message);

    public static implicit operator string(Error error) => error.Message;
}
