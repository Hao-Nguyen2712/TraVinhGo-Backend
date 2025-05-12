// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

namespace TraVinhMaps.Application.External;

public interface ICacheService
{
    Task<T?> GetData<T>(string key);
    Task SetData<T>(string key, T data, TimeSpan? expirationTime = null);
    Task RemoveData(string key);
}
