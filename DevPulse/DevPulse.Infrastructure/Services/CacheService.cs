using DevPulse.Core.Interfaces;
using Microsoft.Extensions.Caching.Memory;

namespace DevPulse.Infrastructure.Services;

public class CacheService : ICacheService
{
    private readonly IMemoryCache _cache;
    public CacheService(IMemoryCache cache) =>_cache = cache;
    
    public Task<T?> GetAsync<T>(string key)
    {
        _cache.TryGetValue(key,out T? value);

        return Task.FromResult(value);
    }

    public Task SetAsync<T>(string key,T value,TimeSpan expiration)
    {
        _cache.Set(key, value,expiration);

        return Task.CompletedTask;
    }

    public void Remove(string key) => _cache.Remove(key);
}
