// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using Microsoft.Extensions.Caching.Distributed;
using Newtonsoft.Json;
using TraVinhMaps.Application.External;

namespace TraVinhMaps.Infrastructure.External;

public class CacheService : ICacheService
{
    private readonly IDistributedCache _distributedCache;

    public CacheService(IDistributedCache distributedCache)
    {
        _distributedCache = distributedCache;
    }

    public async Task RemoveData(string key)
    {
        if (_distributedCache == null)
        {
            throw new InvalidOperationException("Distributed cache is not configured.");
        }

        await _distributedCache.RemoveAsync(key);
    }

    public async Task<T?> GetData<T>(string key)
    {
        var data = await _distributedCache.GetStringAsync(key);
        if (string.IsNullOrEmpty(data))
        {
            return default;
        }

        return JsonConvert.DeserializeObject<T>(data);
    }

    public async Task SetData<T>(string key, T data, TimeSpan? expirationTime)
    {
        var options = new DistributedCacheEntryOptions
        {
            AbsoluteExpirationRelativeToNow = expirationTime ?? TimeSpan.FromMinutes(5),
        };

        await _distributedCache.SetStringAsync(key, JsonConvert.SerializeObject(data), options);
    }
}
