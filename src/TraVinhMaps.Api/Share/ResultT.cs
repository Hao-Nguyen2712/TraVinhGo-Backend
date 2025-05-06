// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

namespace TraVinhMaps.Api.Share;

public class ResultT<T> : Result
{
    public T? Data { get; }

    public ResultT(bool isSuccess, Error error, T? data) : base(isSuccess, error)
    {
        Data = data;
    }
}
